using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fleck
{
    /// <summary>
    /// Provides the basic functions of the WebSocket service.
    /// </summary>
    /// <remarks>
    /// The WebSocketService class is an abstract class.
    /// </remarks>
    public abstract class WebSocketService
    {

        #region Private Fields

        protected IWebSocketConnectionInfo _context;
        protected WebSocketServiceManager _manager;
        protected IWebSocketConnection _websocket;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketService"/> class.
        /// </summary>
        public WebSocketService()
        {
            ID = String.Empty;
        }

        #endregion

        #region Protected Properties


        /// <summary>
        /// Gets the sessions to the <see cref="WebSocketService"/>.
        /// </summary>
        /// <value>
        /// A <see cref="WebSocketServiceManager"/> that contains the sessions to the the <see cref="WebSocketService"/>.
        /// </value>
        protected WebSocketServiceManager Manager
        {
            get
            {
                return _manager;
            }
            set { _manager = value; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the ID of the <see cref="WebSocketService"/> instance.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that contains an ID.
        /// </value>
        public string ID { get; set; }

        #endregion

        #region Private Methods

        //private void onClose(object sender, CloseEventArgs e)
        //{
        //    _sessions.Remove(ID);
        //    OnClose(e);
        //}

        //private void onError(object sender, ErrorEventArgs e)
        //{
        //    OnError(e);
        //}

        //private void onMessage(object sender, MessageEventArgs e)
        //{
        //    OnMessage(e);
        //}

        //private void onOpen(object sender, EventArgs e)
        //{
        //    ID = _sessions.Add(this);
        //    OnOpen();
        //}

        #endregion


        #region Protected Methods

        /// <summary>
        /// Occurs when the inner <see cref="WebSocket"/> receives a Close frame or the Stop method is called.
        /// </summary>
        /// <param name="e">
        /// A <see cref="CloseEventArgs"/> that contains the event data associated with a <see cref="WebSocket.OnClose"/> event.
        /// </param>
        public virtual void OnClose()
        {
        }

        /// <summary>
        /// Occurs when the inner <see cref="WebSocket"/> gets an error.
        /// </summary>
        /// <param name="e">
        /// An <see cref="ErrorEventArgs"/> that contains the event data associated with a <see cref="WebSocket.OnError"/> event.
        /// </param>
        public virtual void OnError()
        {
        }

        /// <summary>
        /// Occurs when the inner <see cref="WebSocket"/> receives a data frame.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MessageEventArgs"/> that contains the event data associated with a <see cref="WebSocket.OnMessage"/> event.
        /// </param>
        public virtual void OnMessage(string _message)
        {
        }

        /// <summary>
        /// Occurs when the WebSocket connection has been established.
        /// </summary>
        public virtual void OnOpen()
        {
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Broadcasts the specified <see cref="string"/> to the clients of every <see cref="WebSocketService"/> instances
        /// in the <see cref="WebSocketService.Manager"/>.
        /// </summary>
        /// <param name="data">
        /// A <see cref="string"/> to broadcast.
        /// </param>
        public void Broadcast(string data)
        {
            _manager.Broadcast(data);
        }



        /// <summary>
        /// Sends a binary data to the client of the <see cref="WebSocketService"/> instance.
        /// </summary>
        /// <param name="data">
        /// An array of <see cref="byte"/> that contains a binary data to send.
        /// </param>
        public void Send(byte[] data)
        {
            _websocket.Send(data);
        }

        /// <summary>
        /// Sends a text data to the client of the <see cref="WebSocketService"/> instance.
        /// </summary>
        /// <param name="data">
        /// A <see cref="string"/> that contains a text data to send.
        /// </param>
        public void Send(string data)
        {
            _websocket.Send(data);
        }


        /// <summary>
        /// Sends a text data to the client of the <see cref="WebSocketService"/> instance
        /// associated with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// A <see cref="string"/> that contains an ID that represents the destination for the data.
        /// </param>
        /// <param name="data">
        /// A <see cref="string"/> that contains a text data to send.
        /// </param>
        public void SendTo(string id, string data)
        {

            //WebSocketService service;
            //if (_sessions.TryGetWebSocketService(id, out service))
            //    service.Send(data);
        }
        #endregion
    }
}
