# Challenges
This repository is intended to be used for learning and education. There are some areas for improvement in this codebase (as with all software). The below are some areas you can improve or extend to solidify and better understand the concepts being taught here.
If you see some areas or have a good idea, feel free to open a PR to add the idea here!

Please do not commit and create PRs with your implementation of these challenges. The challenges are for your own learning and solutions will not be added to the repo.

## Easy(ish)
- Create more levels!
- Add a new tile type (maybe water?)
- Show a more engaging end screen instead of just showing a slightly modified main menu.
- Show random different levels in the background of the main menu instead of only a single static level.
- Upgrade the look and feel of the UI.

## Medium(ish)
- Add new obstacle types (maybe an alligator that chomps?).
- Show a different ending particle system based on the `Par` reached (Perfect / Good / Ok / Fail).
- Bring in the level selection menu to the Game scene (I encountered bugs with ScrollView & UITK, otherwise this should be easy 🙂).
- Add different ball types with different rigidbody configurations and visuals.
- Add background details such as moving clouds or any other scenery you'd like to include.

## Advanced(ish) 
- Add online leaderboards for each level (Consider Unity Gaming Services "UGS" or PlayFab for out of the box solutions for this).
- Gate new balls behind some currency that is awarded based on obtaining higher "Par" ratings.
- Add a "retry shot" mechanic that allows the player to attempt their last putt. Maybe gate this behind some currency.
- Change the lighting at runtime based on level so not all levels have the same ambience. Perhaps some levels are darker. Maybe it's raining on some levels.
- Implement a more robust method of discarding "ghost" collision contacts than the simple override of the normal. Some [suggestions on algorithms can be found here](https://forum.unity.com/threads/experimental-contacts-modification-api.924809/page-2#post-6720985). 
