//////////////////////////////////////////////
//FileName: Controller.cs
//Authors: Dallon Haley and Tyler Allen
//Created On: 11/14/2020
//Description: Handles networking and game logic
/////////////////////////////////////////////
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using SS;

namespace SpreadsheetController
{
    /// <summary>
    /// Handles logic and networking protocol for Spreadsheet
    /// </summary>
    public class Controller
    {
        private static SocketState theServer = null; //socket for connection to server 
        private static string name = ""; //user name of local client 
        private static int id;

        // Event to update wiew with new info from server
        public delegate void DataHandler(List<string> spreadsheets, SocketState state);
        public delegate void DataEvent(Spreadsheet spreadsheet);
        public event DataHandler FileSelect;
        public event DataEvent Update;

        // Event to update view of network errors
        public delegate void ErrorHandler(string err);
        public event ErrorHandler Error;

        private List<string> spreadsheets = new List<string>();

        /// <summary>
        /// Sets up initial connection with server 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="playerName"></param>
        /// <param name="theWorld"></param>
        public void Connect(string server, string userName)
        {
            name = userName;
            Networking.ConnectToServer(OnConnect, server, 1100);
        }

        /// <summary>
        /// On Network Action method to be called once connection is set up. 
        /// Method sends user name to server and initiates GetData event loop 
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error("Error while connecting."); //update view of error
                return;
            }

            theServer = state;
            Networking.Send(state.TheSocket, name + "\n"); //update server with user name 
            state.OnNetworkAction = OnRecieveFile; //set up OnNetworkAction for handshake protocol 

            Networking.GetData(state); //initiate receive network loop 
        }

        /// <summary>
        /// On Network Action method to be called with inital server messages arrive: spreadsheet information
        /// If error occured during message retrieval, the view is notified. 
        /// GetData event loop is continued with OnReceive as the new OnNetworkAction delegate
        /// </summary>
        /// <param name="state"></param>
        private void OnRecieveFile(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error("Error while retrieving setup!"); //update view of error 
                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            foreach(String name in parts)
            {
                if (name.Equals("\n"))
                    break;             
                spreadsheets.Add(name);
            }

            for(int i = 0; i < parts.Length; i++)
            //{
                state.RemoveData(0, parts[i].Length); // remove already handled lines from server string builder 
            //}

            FileSelect(spreadsheets, state);

            foreach (string name in parts)
            {
                Console.Write(name);
            }

            //state.RemoveData(0, parts.Length); // remove first two (already handled) lines from server string builder
            state.OnNetworkAction = OnReceiveSpreadsheet; //change OnNetworkAction for normal JSON server communications 
            Networking.GetData(state);
        }

        /// <summary>
        /// On Network Action method to be called when message(clientID) has been receieved. 
        /// If error occured, view is updated. 
        /// </summary>
        /// <param name="state"></param>
        private void OnReceiveSpreadsheet(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error("Error while retrieving Client ID"); //update view of error 
                return;
            }

            Spreadsheet spreadsheet = new Spreadsheet();
            
            string totalData = state.GetData();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            foreach (string part in parts)
            {
                state.RemoveData(0, part.Length); // remove already handled lines from server string builder 
                if (part.Length == 0)
                    continue;
                if (part[part.Length - 1] != '\n')
                    break;

                if (part.Equals("3\n"))
                {
                    state.OnNetworkAction = OnReceive;                  
                    break;
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine(totalData);
                    var result = JsonConvert.DeserializeObject<JToken>(part);
                    System.Diagnostics.Debug.WriteLine(result["messageType"]);
                    if (result["messageType"].ToString() == "cellUpdated")
                    {
                        spreadsheet.SetContentsOfCell(result["cellName"].ToString(), result["contents"].ToString());
                    }

                    else if (result["messageType"].ToString() == "cellSelected")
                    {
                        spreadsheet.SetSelected(result["cellName"].ToString(), int.Parse(result["selector"].ToString()), result["selectorName"].ToString());   
                    }
                }
            }
           
            Update(spreadsheet); // Update view
            Networking.GetData(state);
        }

        /// <summary>
        /// On Network Action method to be called when message has been receieved. Method calls ProcessMessages and continues GetData event loop. 
        /// If error occured, view is updated. 
        /// </summary>
        /// <param name="state"></param>
        private void OnReceive(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error("Error while receiving data!"); //update view of error 
                return;
            }

            ProcessMessages(state); //convert JSON data to spreadsheet cell objects
            
            Networking.GetData(state);
        }

        /// <summary>
        /// Parses JSON data and updates the spreadsheet and notifies view of changes
        /// </summary>
        /// <param name="state"></param>
        private void ProcessMessages(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])"); //splits string by new lines 

            // After connection to server, print whatever we get
            // Debug.WriteLine();
            Console.WriteLine(parts[0]);

            Networking.Send(state.TheSocket, "Test\n");
        }

        public List<String> GetSpreadsheets()
        {
            return null;
        }

        public void SendFileSelect(string file, SocketState state)
        {
            Networking.Send(state.TheSocket, file + "\n");
        }
    }
}
