import { Stack, Tooltip } from "@mui/material";
import React from "react";
import GetTime from "../../Utils/GetTime";

const SentMessage = ({ message, index }) => {
  
  console.log(message);
  console.log(GetTime(message.timestamp));
  
  return (
    <Tooltip title={GetTime(message.timestamp)} placement="left" arrow>
      <Stack
        key={index}
        p={1}
        borderRadius={"15px"}
        color={"white"}
        bgcolor={"#03AC13"}
        alignSelf={"end"}
        maxWidth={{ xs: "270px", sm: "400px", md: "500px", lg: "700px" }}
        sx={{
          wordWrap: "break-word",
          overflowWrap: "break-word",
        }}
      >
        {message.content}
      </Stack>
    </Tooltip>
  );
};

export default SentMessage;
