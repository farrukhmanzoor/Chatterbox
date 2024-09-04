import React, { useContext, useEffect, useRef } from "react";
import { useState, createContext } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import AuthContext from "./AuthProvider";
import AlertContext from "./AlertProvider";
import axios from "axios";
import GetTime from "../Utils/GetTime";

export const ChatContext = createContext();

const ChatContextProvider = ({ children }) => {
  const [selectedUser, setSelectedUser] = useState(null);
  const selectedUserRef = useRef(selectedUser);
  const [messages, setMessages] = useState([]);
  const [connectionState, setConnectionState] = useState(null);
  const { token, user } = useContext(AuthContext);
  const { openAlert } = useContext(AlertContext);
  const [isThereMoreMessages, setIsThereMoreMessages] = useState(false);
  const [activeUsers, setActiveUsers] = useState([]);
  const [newMessage, setNewMessage] = useState(null);

  useEffect(() => {
    const fetch = async () => {
      const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:44307/chathub", {
          accessTokenFactory: () => token,
        })
        .configureLogging(LogLevel.Debug)
        .build();
      connection.on("ReceiveMessage", async (receivedMessage, username) => {

        await axios.post(
          "https://localhost:44307/api/Message/acknowledgeMessage",
          receivedMessage,
          {
            headers: {
              Accept: "application/json",
              "Content-Type": "application/json",
              Authorization: `bearer ${token}`,
            },
          }
        ).catch(error => console.error("Error acknowledging message:", error));    



        const currentSelectedUser = selectedUserRef.current;
        if (
          currentSelectedUser &&
          receivedMessage.userId === currentSelectedUser.id
        ) {
          setMessages((messages) => [...messages, receivedMessage]);
        } else {
          openAlert("success", `you received a message from ${username}`);
          setNewMessage(receivedMessage);
        }
      });
      connection.on("ReceiveActiveUsers", (newActiveUsers) => {
        setActiveUsers(newActiveUsers);
      });
      await connection
        .start()
        .then(() => {
          console.log("SignalR connection started.");
        })
        .catch((error) => {
          console.error("Error starting SignalR connection:", error);
        });

      setConnectionState(connection);
    };
    fetch();
  }, []);

  useEffect(() => {
    selectedUserRef.current = selectedUser;
    setMessages([]);
    selectedUser && loadMessages(true);
  }, [selectedUser]);

  const loadMessages = async (isNewUser) => {
    console.log("loading Messages");
    var data = {
      pageDate: messages[0] && !isNewUser ? messages[0].timestamp : null,
      pageSize: 13,
      firstUserId: user.id,
      secoundUserId: selectedUser.id,
    };
    await axios
      .get("https://localhost:44307/api/messages/getMessages", {
        params: data,
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          Authorization: `bearer ${token}`,
        },
      })
      .then((response) => {
        console.log("response", response.messages);
        isNewUser
          ? setMessages(response.data.messages)
          : setMessages([...response.data.messages, ...messages]);
          setIsThereMoreMessages(response.data.isThereMore);
      });
  };

  const handleSendMessage = async (message) => {
   
    if (selectedUser && message !== "") {
      var newMessage = {
        id:0,
        userId: user.id,
        recipientId: selectedUser.id,
        timestamp:new Date(),
        content: message,
       
      };
      setMessages([...messages, newMessage]);
      setNewMessage(newMessage);
        
      await connectionState
        .invoke("SendMessageToUser", (newMessage))
        .catch((error) => {
          console.error("Error sending message:", error);
        });
    }
  };

  return (
    <ChatContext.Provider
      value={{
        messages,
        selectedUser,
        setSelectedUser,
        handleSendMessage,
        isThereMoreMessages,
        loadMessages,
        activeUsers,
        newMessage,
      }}
    >
      {children}
    </ChatContext.Provider>
  );
};

export default ChatContextProvider;
