import { Divider, Stack, styled } from "@mui/material";
import React, { useState } from "react";
import LeftSidebarHeader from "./Header";
import RecentChats from "./RecentChats";
import MySearchTextField from "../Inputs/MySearchTextField";
import SearchResult from "./SearchResult";

const StyledStack = styled(Stack)(({ theme }) => ({
  backgroundColor: theme.palette.background.paper,
  padding: "15px 10px",
  height: "100%",
}));

const LeftSidebar = () => {

  const [pageNumber, setPageNumber] = useState(1);
  const [searchTerm, setSearchText] = useState("");
  const onSearchTextChange = (event) => {
    setPageNumber(1);
    const value = event.target.value;
    setSearchText(value);
  };
  return (
    <StyledStack >
      <LeftSidebarHeader />
      <Divider />
      <MySearchTextField
        value={searchTerm}
        onChange={onSearchTextChange}
        placeholder={"search people"}
      />
       <RecentChats displayState={searchTerm !== ""} />
      {searchTerm !== "" && (
        <SearchResult
          pageNumber={pageNumber}
          setPageNumber={setPageNumber}
          searchTerm={searchTerm}
        />
      )} 
    </StyledStack>
  );
};

export default LeftSidebar;
