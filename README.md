<h1 align="center">Switchy Shapes</h1>

<p align="center">
	<a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.ShapeSwitcher" target="_blank">
	<img width="180" height="80" 
	src="https://play.google.com/intl/en_us/badges/static/images/badges/en_badge_web_generic.png" alt="">
	</a>
</p>

### Summary:
It has been a great learning experience and I'm quite happy with the final product. All game programming, UI, UX, sound design, graphics, store management (and so on...) was done by yours truly.

<br>

### Code Samples:

* I would share the whole unity project, but it could pose security risks since the game in on the google playstore.
* _LevelLocker.cs_
	*Uses the player's current level to decide which levels are available to play.
* _LevelPreprocessor.cs_
	* Gathers the needed date to set up a level correctly. This includes things like the configuration of the shape columns, height gap between shapes, and the flow speed of the shapes.
* _LevelSpawner.cs_
	* Takes information from LevelPreprocessor.cs and spawns flowing shapes before the game starts, sets their 
<br>

### Technical Details:.
* 2D mobile game
* Built from scratch using the Unity Game Engine and C# scripting
* Close to 4000 lines of C# code across 20 scripts
* Uses Unity Services like Analytics and Monetization
* User-interface scales to any display
* "Player" system to securely save/load player data
* Game updates are easily implemented and retain player data

<br>

### Game Features:
* Available on Android (coming to iOS soon)
* 90 carefully created levels

<br>

### Screenshots:
* ![Screenshot](/images/combinedScreenshots.jpg)</li>

<br>


### Development Overview
* Before I started planning, I made sure that this was a project that I was capable of completing. Once I decided that, I moved on to turning the idea into reality.
* I started this project by planning out the core gameplay design. Things like goals of the game, the user's experience and the user interface.
* Next I planned out a modular parts of the game from a top down approach. First, I started with a global script which would store data between scene changes. I used the method "DontDestroyOnLoad(this);" on the script, which would keep it's instance between scenes. Any other scripts I needed could be added to that GameObject, such as the scripts for advertisements, menus and audio. 
* After the global object was established, I focused on the core game event system which would handle physics and collisions for the flowing shapes, the catcher buttons' onClick listeners, the menu's needed for screens like pause, win and game over and

I had to establish things like flow object physics, hitbox colliders for the shapes and catchers, 


### Web Links:
* <a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.ShapeSwitcher" target="_blank">Google play store</a>
* <a href="https://www.harjindercheema.com" target="_blank">My Website</a>
								
