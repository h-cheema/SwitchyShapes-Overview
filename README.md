<h1 align="center">Switchy Shapes</h1>

<p align="center">
	<a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.ShapeSwitcher" target="_blank">
	<img width="180" height="80" 
	src="https://play.google.com/intl/en_us/badges/static/images/badges/en_badge_web_generic.png" alt="">
	</a>
</p>

### Summary:
Switchy Shapes is a 2D mobile game built from scratch using the Unity Game Engine and C# scripting. It has been a great learning experience and I'm quite happy with the final product. All game programming, UI, UX, sound design, graphics, store management (and so on...) was done by myself.

<br>

### Code Samples:

* All code commenting and documentation is from the project's original files. I would have never been able to finish this project if I never documented my code.

* _<a href="https://github.com/h-cheema/SwitchyShapes-Overview/blob/master/LevelLocker.cs" target="_blank">LevelLocker.cs</a>_
	* Uses the player's current level to decide which levels are available to play. It instantiates level buttons and locks/disables specific ones which are not completed yet. It also uses a coroutine to make a button (current level button, next page button or previous page button) flash to help the player know what to do.

* _<a href="https://github.com/h-cheema/SwitchyShapes-Overview/blob/master/LevelPreprocessor.cs" target="_blank">LevelPreprocessor.cs</a>_
	* Gathers the needed date to set up a level correctly. This includes things like the configuration of the shape columns, height gap between shapes, and the flow speed of the shapes.

* _<a href="https://github.com/h-cheema/SwitchyShapes-Overview/blob/master/LevelSpawner.cs" target="_blank">LevelSpawner.cs</a>_
	* Takes information from LevelPreprocessor.cs and spawns flowing shapes before the game starts.
<br>

### Technical Details:
* 2D mobile game
* Built from scratch using the Unity Game Engine and C# scripting
* Close to 4000 lines of C# code across 20 scripts
* Uses Unity Services like Analytics and Monetization
* User-interface scales to any display
* "Player" system securely saves/loads player data
* Game updates are easily implemented and retain player data

<br>

### Game Features:
* Available on Android (coming to iOS soon)
* 90 carefully created levels

<br>

### Screenshots:
* ![Screenshot](/images/combinedScreenshots.jpg)</li>

<br>

### Web Links:
* <a href="https://play.google.com/store/apps/details?id=com.TeraKeySoftware.ShapeSwitcher" target="_blank">Google play store</a>
* <a href="https://www.harjindercheema.com" target="_blank">My Website</a>
								
