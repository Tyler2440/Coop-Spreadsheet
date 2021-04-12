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

namespace SpreadsheetController
{
    /// <summary>
    /// Handles logic and networking protocol for Spreadsheet
    /// </summary>
    public class Controller
    {
        private static SocketState theServer = null; //socket for connection to server 
        private static string name = ""; //user name of local client 

        // Event to update wiew with new info from server
        public delegate void DataHandler();
        //public delegate void DataHandler(Spreadsheet spreadsheet);
        public event DataHandler DataEvent;

        // Event to update view of network errors
        public delegate void ErrorHandler(string err);
        public event ErrorHandler Error;

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
            state.OnNetworkAction = OnHandshake; //set up OnNetworkAction for handshake protocol 

            Networking.GetData(state); //initiate receive network loop 
        }

        /// <summary>
        /// On Network Action method to be called with inital server messages arrive: spreadsheet information
        /// If error occured during message retrieval, the view is notified. 
        /// GetData event loop is continued with OnReceive as the new OnNetworkAction delegate
        /// </summary>
        /// <param name="state"></param>
        private void OnHandshake(SocketState state)
        {
            if (state.ErrorOccured)
            {
                Error("Error while retrieving setup!"); //update view of error 
                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");



            state.RemoveData(0, parts[0].Length + parts[1].Length); // remove first two (already handled) lines from server string builder 
            //DataEvent(spreadsheet); //update view 
            state.OnNetworkAction = OnReceive; //change OnNetworkAction for normal JSON server communications 
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
            Console.WriteLine(parts.ToString());
        }
    }
}
