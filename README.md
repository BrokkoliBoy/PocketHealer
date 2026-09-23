# Pocket Healer

## What is it?
A simple simulation of a World of Warcraft like healing game. Only the healing part of WoW is implemented. You have several healing spells that you use to heal your group while you group is killing a boss.

## State
Work paused. I can continue to add conent if I ever want to.<br><br>
Technical features: 90% done, if more content is created and more spells have to be implemented, that would come on top.<br><br>
Content: 3/5 bosses finished, 6/8 healing spells finished.

## Goal
I played a mobile game which conceptually was basically the same as this project. I had fun playing it and I also generally have fun healing in games, so I figured why not create my own game in that style. This is a pure fun project with the intention of making it extra hard for players to beat. Also, I wanted to learn what challanges come with implementing this kind of 2D game.

## Concept
The game consists of different encounters in which your group has to fight a boss by passively doing damage. Meanwhile the group takes damage which you have to heal against by using your healing spells. The implemented spells roughly resemble the spells of the WoW Priest class.<br><br>

Spells cost mana and inflict direct healing or temporary state effects (aka buffs) that apply healing, absorbtion shields or other things like increased damage dealt or reduced damage taken. The player may only bring a selection of 5 spells into encounters. They can manage which spells to bring, which slot in the action bar they are in and which hotkey that slot is assigned to. The player starts with two basic healing spells and whenever they beat an encounter, new spells are added.<br><br>

Encounters are singular boss fights. Encounters come in 2 different difficulties: Normal and Heroic (with tougher numbers to beat). Once a boss is beat, the next difficulty is unlocked for that boss.

## Features
- fully working healing simulation
- editor* for spells
- editor* for encoutners and bosses
  - includes group size and group setup
- bosses in 2 difficulties
- skill learning after bosses
- drag and drop spells into action bars

## Learnings

### Using a UI plugin
This was my first time using a UI plugin ('Doozey'). I figured I'd check out how it is using a 2D UI plugin. I throughoutly checked out most of the features and implemented them into the game. There were some quite useful features such as natively implemented UI transitions, sounds and event triggers, but overall I felt like the plugin gave me 'too much' stuff that I didn't use, but still needed to maintain. I am not sure if I would say that I could have gotten the same results faster if I implemented it in my own way. But I did learn that using UI plugins (or at least the one I use) requires some works and if the features of the plugin is not utilized, it might be better to go for a different approach. I guess it's the same as with all tools: Find the right tool for the right task.

### Saving & loading
This was not the first time I had to permanently store information on computer. But the save & load feature in this project was a bit more complex than what I knew. I also made the mistake of only implementing the save & load feature very late in development. I did have it in mind when developing, but the one time it kind of screwed me over was when I had to store which of the spells the user has in which action bar slot. I had no way of identifying spells since spells are stored in prefabs. The implementation wasn't hard, all it took was to add an identifier to each of the spells which the save & load script uses to set the spells into the action bars, but I did give it some thought before I implemented it, because I wanted to find an elegant solution.

### One-scene Project
I decided to implement the whole game in just one scene. This gives the benefit of completely avoiding loading times. I wanted the game to feel like a small mini game and I thought that having no waiting times at any point would feel great for such a game. Yet, this also means that I have to go without the benefits of resetting the scene and have a fresh go with fresh GameObjects at every scene start. I learned that doing everything in one scene requires getting a good understanding of the life cycle of pretty much all GameObjects and scripts and it taught me to think of each and every component of the project as a reusable asset (in contrast to a throwaway asset). I think I managed to get it done rather well and I would definetely consider going for a one-scene project again, given the benefits outweight the drawbacks.
