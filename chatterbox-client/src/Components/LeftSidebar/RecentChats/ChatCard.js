import { Stack, Typography,Badge } from "@mui/material";
import React,{ useState }  from "react";
import MyAvatar from "../../UserCard/MyAvatar";
import { GetShortTime } from "../../Utils/GetTime";
import styled from "@emotion/styled";

const StyledTypography = styled(Typography)({
  fontWeight: "bold",
});

const StyledTypography2 = styled(Typography)({
  fontSize: "14px",
  overflow: "hidden",
  textOverflow: "ellipsis",
  whiteSpace: "nowrap",
  maxWidth: "80%",
});

const ChatCard = ({ chat, onChatSelect }) => {
  const [unreadCount, setUnreadCount] = useState(chat.unreadCount);

  const handleClick = () => {
    onChatSelect(chat);
    setUnreadCount(0); // Reset unread count on selection
  };

  return (
    <Stack
      direction={"row"}
      alignItems={"center"}
      justifyContent={"space-between"}
      width={"100%"}
      height={"60px"}
    >
      <Stack direction={"row"} spacing={2} alignItems={"center"} width={"60%"}>
        <MyAvatar user={chat.user} />
        <Stack width={"80%"}>
          <StyledTypography variant="h6" fontSize={{ xs: "14px", md: "16px" }}>
            {chat.user.username}
          </StyledTypography>
          <StyledTypography2>{chat.lastMessage.content}</StyledTypography2>
        </Stack>
      </Stack>
      <Badge
                badgeContent={chat.unReadCount}
                color="primary"
                invisible={chat.unReadCount === 0}
                overlap="circular"
                onClick={handleClick}
                style={{ cursor: "pointer" }}
              ></Badge>
      <Typography fontSize={{ xs: "10px", md: "14px" }}>
        {GetShortTime(chat.lastMessage.timestamp)}
      </Typography>
    </Stack>
  );
};

export default ChatCard;
