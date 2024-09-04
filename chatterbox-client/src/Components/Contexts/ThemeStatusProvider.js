import React, { createContext, useContext, useEffect, useState } from "react";
import AuthContext from "./AuthProvider";


export const ThemeStatusContext = createContext("");

const ThemeStatusContextProvider = ({ children }) => {
  const [isDarkTheme, setIsDarkTheme] = useState(() => {
    const storedValue = false;// localStorage.getItem("isDarkTheme");
    return storedValue ? JSON.parse(storedValue) : false;
  });

  const { user, token } = useContext(AuthContext);

  
  useEffect(() => {
    if (user) {
      setIsDarkTheme(user.isDarkTheme);
      localStorage.setItem("isDarkTheme", JSON.stringify(user.isDarkTheme));
    }
  }, [user]);

  const toggleTheme = () => {
    setIsDarkTheme((prevTheme) => !prevTheme);
  };

  return (
    <ThemeStatusContext.Provider
      value={{
        isDarkTheme,
        toggleTheme,
      }}
    >
      {children}
    </ThemeStatusContext.Provider>
  );
};

export default ThemeStatusContextProvider;
