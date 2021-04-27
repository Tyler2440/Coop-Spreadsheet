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
        private int id;
        private bool finishedHandshake = false;

        // Event to update wiew with new info from server
        public delegate void DataHandler(List<string> spreadsheets);
        public delegate void CellSelectionHandler(string cellName, int ID, string username);
        public delegate void ServerErrorHandler(string message);
        public delegate void RequestErrorHandler(string cellname, string message);
        public delegate void ChangeContentsHandler(string cellName, string contents);
        public event ChangeContentsHandler ChangeContents;
        public event ServerErrorHandler ServerError;
        public event RequestErrorHandler RequestError;
        public event DataHandler FileSelect;
        public event CellSelectionHandler cellSelection;

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

            foreach (String name in parts)
            {
                if (name.Equals("\n"))
                    break;
                spreadsheets.Add(name);
            }

            for (int i = 0; i < parts.Length; i++)
                //{
                state.RemoveData(0, parts[i].Length); // remove already handled lines from server string builder 
            //}

            FileSelect(spreadsheets);

            foreach (string name in parts)
            {
                Console.Write(name);
            }

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

            string totalData = state.GetData();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            foreach (string part in parts)
            {
                state.RemoveData(0, part.Length); // remove already handled lines from server string builder 
                if (part.Length == 0)
                    continue;
                if (part[part.Length - 1] != '\n')
                    break;

                if (!finishedHandshake)
                {
                    if (int.TryParse(part.Substring(0, part.Length - 1), out id))
                    {
                        finishedHandshake = true;
                        break;
                    }
                }

                var result = JsonConvert.DeserializeObject<JToken>(part);

                if (result["messageType"].ToString() == "cellUpdated")
                {
                    //spreadsheet.SetContentsOfCell(result["cellName"].ToString(), result["contents"].ToString());
                    ChangeContents(result["cellName"].ToString(), result["contents"].ToString());
                }

                else if (result["messageType"].ToString() == "cellSelected")
                {
                    //spreadsheet.SetSelected(result["cellName"].ToString(), int.Parse(result["selector"].ToString()), result["selectorName"].ToString());
                    if (Int32.Parse(result["selector"].ToString()) != id)
                    {
                        cellSelection(result["cellName"].ToString(), int.Parse(result["selector"].ToString()), result["selectorName"].ToString());
                    }
                    
                }

                else if (result["messageType"].ToString() == "requestError")
                {
                    RequestError(result["cellName"].ToString(), result["message"].ToString());
                }

                else if (result["messageType"].ToString() == "serverError")
                {
                    ServerError(result["message"].ToString());
                }

            }
            Networking.GetData(state);
        }

        /// <summary>
        /// Sends clients spreadsheet selection upon joining server 
        /// </summary>
        /// <param name="file">spreadsheet name</param>
        /// <param name="state">socket state</param>
        public void SendFileSelect(string file)
        {
            Networking.Send(theServer.TheSocket, file);
        }

        /// <summary>
        /// Sends clients cell selection when they click on cell
        /// </summary>
        /// <param name="cellName">selected cell name</param>
        public void SendCellSelection(string cellName)
        {
            if (finishedHandshake)
            {
                string message = "{ requestType: \"selectCell\", cellName: \"" + cellName + "\" }" + "\n";
                Networking.Send(theServer.TheSocket, message);
            }
        }

        /// <summary>
        /// sends client request to change contents of cell 
        /// </summary>
        /// <param name="cellName">cell name</param>
        /// <param name="contents">contents</param>
        public void RequestCellEdit(string cellName, string contents)
        {
            if (finishedHandshake)
            {
                string message = "{ requestType: \"editCell\", cellName: \"" + cellName + "\", contents: \"" + contents + "\" }" + "\n";
                Networking.Send(theServer.TheSocket, message);
            }
        }

        /// <summary>
        /// sends client request to undo change
        /// </summary>
        public void RequestUndo()
        {
            if (finishedHandshake)
            {
                string message = "{ requestType: \"undo\" }" + "\n";
                Networking.Send(theServer.TheSocket, message);
            }
        }

        /// <summary>
        /// sends client request to revert cell change 
        /// </summary>
        /// <param name="cellName">cell name to revert</param>
        public void RequestRevert(string cellName)
        {
            if (finishedHandshake)
            {
                string message = "{ requestType: \"revertCell\", cellName: \"" + cellName + "\" }" + "\n";
                Networking.Send(theServer.TheSocket, message);
            }
        }

    }
}
