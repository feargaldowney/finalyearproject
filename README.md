# HelpingTimToTalk
This project acts as a speech therapy intervention and practice tool. It reinvents gamification in education by embedding learning into real gameplay, instead of typical highscores and leaderboards.

Helping Tim to Talk utilizes Unity, Google's Speech-to-text API, FireBase & MailGun API in order to offer children a more immediate and enjoyable form of speech therapy intervention.
This is a 2D mobile game, developed for android users.

GAMEPLAY:

The game consists of 5 distinct levels. 
Much like other platformers such as 'Super Mario Bros' you must run from left to right along the level.
In each level there are two forms of collectibles: Cherries & Pineapples.

Cherries act as a score counter, similar to currency. Although not implemented as such.

Pineapples are challenge items.
Once a user jumps and collects a pineapple, the game temporarily freezes. 
A speech prompt appears on screen. a word from that levels unique dataset. More on this further down.
The user has a few seconds to say the word on screen. Once audio input is detected, the game resumes and the user is rewarded with a bonus jump. 
Encouraging the user to collect pineapples.

Each level has an array of unique speech prompts. 
This is so that each level focuses on different phonetic challenges to accurately gauge which sounds the user struggles with.
For example one level might focus on 'TH' sounds such as 'that' or 'this' where another level will focus on 's' sounds like 'slip' or 'smile'.

ANALYZING USER ACCURACY:

Each time audio input is detected. the users speeh is recorded. Their speech is translated into text via the use of Googles Speech to Text with the help of minor contextual hints.
the users input is then compared to the prompt on screen in real time.

By the end of the level the users accuracy will have been determined.
All words that the user gets incorrect are appended to an array of incorrect words. 
This array of incorrect words is emailed to the parent or guardian of the user, along with an accuracy report detailed with suggestions on speech fluency intervention and their accuracy on the specific phonetic challenge of that level. 

This then allows the user to practice the words they struggled on with their parents.


Feel free to test the app yourself on an android device. 
Find more information along with the apk in my google drive:
https://drive.google.com/drive/folders/1AdnWaEhcCy4CFHfg5Zn8AFaT1cycGLLD?usp=sharing

