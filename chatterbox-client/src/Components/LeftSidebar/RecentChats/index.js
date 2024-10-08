import React, { useContext, useEffect, useState } from "react";
import AuthContext from "../../Contexts/AuthProvider";
import axios from "axios";
import {
  Box,
  CircularProgress,
  List,
  ListItem,
  ListItemButton,
} from "@mui/material";
import { ChatContext } from "../../Contexts/ChatProvider";
import ChatCard from "./ChatCard";

const RecentChats = ({ displayState }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [chats, setChats] = useState([]);
  const { token, user } = useContext(AuthContext);
  const { selectedUser, setSelectedUser, newMessage } = useContext(ChatContext);

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      const url =`https://localhost:44307/api/Messages/getRecentChats?userId=${user.id}`;
      console.log("URL", url);
      await axios
        .get(url, {
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
            Authorization: `bearer ${token}`,
          },
          
        })
        .then((response) => {         
          setChats(response.data);
        });
      setIsLoading(false);
    };
    fetchData();
  }, [token, user]);

  useEffect(() => {
    const fetchData = async () => {
      const { userId, recipientId } = newMessage;
      const chatUserId = userId === user.id ? recipientId : userId;
      const chatIndex = chats.findIndex((chat) => chat.user.id === chatUserId);
      const updatedChats = [...chats];
      if (chatIndex !== -1) {
        const newChat = {
          ...updatedChats[chatIndex],
          lastMessage: newMessage,
          unReadCount: userId === user.id ? updatedChats[chatIndex].unReadCount : updatedChats[chatIndex].unReadCount + 1,
        };
        updatedChats.splice(chatIndex, 1);
        updatedChats.unshift(newChat);
        
      } else {
        const chatUser = await getUserById(chatUserId);
        const newChat = { 
          user: chatUser, 
          lastMessage: newMessage,
          unReadCount: userId === user.id ? 0 : 1, 
        };
        updatedChats.unshift(newChat);
      }
      setChats(updatedChats);
    };
    if (newMessage) {
      fetchData();
    }
  }, [newMessage, user.id]);

  const getUserById = async (userId) => {
    var newUser;
    await axios
      .get(`https://localhost:44307/api/user/getUserById?userId=`+userId, {
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          Authorization: `bearer ${token}`,
        },
        //params: { userId: userId },
      })
      .then((response) => {
        newUser = response.data;
      });

    return newUser;
  };

  const handleChatSelect =  async (chat) => {
    setSelectedUser(chat.user);
  
    const updatedChats = chats.map(c => 
      c.user.id === chat.user.id 
        ? { ...c, unReadCount: 0 } 
        : c
    );
  
    setChats(updatedChats);
  
    await axios.post(
      "https://localhost:44307/api/Messages/markAsRead",
      {
        userId: user.id,
        recipientId: chat.user.id ,
      },
      {
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
          Authorization: `bearer ${token}`,
        },
      }
    );    
  };

  return isLoading ? (
    <Box flex={4}>
      <CircularProgress color="inherit" size={16} />
    </Box>
  ) : (
    <List
      sx={{
        "&& .Mui-selected": {
          backgroundColor: "#75757520",
        },
        display: displayState ? "none" : "block",
      }}
    >
      {chats.map((chat, index) => {
        return (
          <ListItem
            disablePadding
            selected={selectedUser?.id === chat.user.id}
            key={index}
          >
            <ListItemButton
              key={index}
              onClick={() => handleChatSelect(chat)}
            >
              <ChatCard chat={chat}  onChatSelect={handleChatSelect}/>
              
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
};

export default RecentChats;
