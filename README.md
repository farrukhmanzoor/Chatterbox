Chatterbox A real-time chat application

About:

This is a real-time chat application built using React (Create React App on the front) and C#, SignalR,Core Backend.

Users can sign up, search, and message other users in real-time.
Tech Stack:

    SignalR
    .Net core 8
    React (Create React App)
    SASS
    Dockers

Before proceeding, please ensure you have the following software installed on your computer.

    Node
    VS Code
    Git command line tools
	Dockers(for containers)

Useful links

    Download Git CLT - Windows: https://git-scm.com/download/windows 
	Mac: https://git-scm.com/download/mac
    Download Node - https://nodejs.org/en/
    Download VSCode - https://code.visualstudio.com/

**Getting started**

Please fork a copy of this repository. Forking a repository allows you to freely experiment with changes without affecting the original project. Alternatively, download or clone the master branch.
Download & Install Dependencies on your machine

Clone the repo to your machine.

    git clone https://github.com/farrukhmanzoor/Chatterbox.git

Launch the frontend

	Open a new terminal window and navigate inside the 'chatterbox-client' folder, as you will need to keep the backend running in the background.
    cd <../path/to/Frontend> 

 Run
 
     npm install

Run the start script

     npm start

Your app should be running on: http://localhost:3000

Backend

Build and run the application using build command
    
	dotnet build Chatterbox.Web



For running application containers install dockers 
Run command

    docker compose up

