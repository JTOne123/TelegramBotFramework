﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotBase.Base
{
    public class MessageClient
    {

        public String APIKey { get; set; }

        public Telegram.Bot.TelegramBotClient TelegramClient { get; set; }

        private EventHandlerList __Events { get; set; } = new EventHandlerList();

        private static object __evOnMessage = new object();

        private static object __evCallbackQuery = new object();


        public MessageClient(String APIKey)
        {
            this.APIKey = APIKey;
            this.TelegramClient = new Telegram.Bot.TelegramBotClient(APIKey);

            Prepare();
        }

        public MessageClient(String APIKey, Telegram.Bot.TelegramBotClient Client)
        {
            this.APIKey = APIKey;
            this.TelegramClient = Client;

            Prepare();
        }


        public void Prepare()
        {
            this.TelegramClient.Timeout = new TimeSpan(0, 0, 30);


            this.TelegramClient.OnMessage += TelegramClient_OnMessage;
            this.TelegramClient.OnCallbackQuery += TelegramClient_OnCallbackQuery;

        }


        private async void TelegramClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            //Skip empty messages by default
            if (e.Message == null)
                return;

            try
            {
                var mr = new MessageResult(e);
                mr.Client = this;
                OnMessage(mr);
            }
            catch
            {

            }
        }

        private async void TelegramClient_OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            try
            {
                var ar = new MessageResult(e);
                ar.Client = this;
                OnAction(ar);
            }
            catch
            {

            }
        }


        #region "Events"

        public event EventHandler<MessageResult> Message
        {
            add
            {
                this.__Events.AddHandler(__evOnMessage, value);
            }
            remove
            {
                this.__Events.RemoveHandler(__evOnMessage, value);
            }
        }

        public void OnMessage(MessageResult result)
        {
            (this.__Events[__evOnMessage] as EventHandler<MessageResult>)?.Invoke(this, result);
        }

        public event EventHandler<MessageResult> Action
        {
            add
            {
                this.__Events.AddHandler(__evCallbackQuery, value);
            }
            remove
            {
                this.__Events.RemoveHandler(__evCallbackQuery, value);
            }
        }

        public void OnAction(MessageResult result)
        {
            (this.__Events[__evCallbackQuery] as EventHandler<MessageResult>)?.Invoke(this, result);
        }


        #endregion


    }
}