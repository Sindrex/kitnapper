# kitnapper
Repository for the game project The Color Kitnapper.
https://store.steampowered.com/app/4154720/The_Color_Kitnapper/

Started feb 2025.
First commit july 2025.
Released 04.06.2026.

# Event notes
If I change name of events, or replace events with new ones, it will break existing save files.

# Dialogue rules
* Dialogue: Max 135 chars
* Choices: Max 10 + 10 chars

# SFX rules
* Max amplitude:-8.1Db

# TODO:
* Make reddit posts
  * IndieGaming https://www.reddit.com/r/IndieGaming/
    * Parody face reveal with Kira "I just released my game!" (trend) (oversees worker hooman)
* Upload music tracks to youtube:
  * Forest (Paw Prints)
  * Town (Feline the City)
  * Cattelatte Theme
  * Demo 6: Poppy's Theme
  * Demo 9: Purrfect Ending
  * Demo 11: Brother's Duet
  * Brother's Duet (live)
* Make 'Live' version Brother's Duet
  * Wait for ChrisHP to record piano -> make Live versjon
* Other references if time -> Outer reality
  * SRC 1/2 (3?) -> A computer prop with dialogue?
* Playtest after fixing all bugs!!!
* Speedrun the game (~1 month after release)
* Add one or two gifs to Steam description.

* Test fixes:
  * Possible issue with music when fading between, and fades triggered rapidly. Maybe queue music if so?
  * When in windowed, text is underflowing border
    * Hard to fix. Dialogue text shrinks at intervals, while border does seamlessly
  * Rock flinched teleport slightly
    * Still issue. Flinched to the left.
  * Issue with Dialogue log if you move up when not enough entries.
  * Maybe add to Kudos ball OOC text that says "this will trigger the credits"
  * Change Manekineko name to Ossan in credits
      * Manekineko idle is different in credits, name is different and chest tag is different. Copy character again
  * Golden balls dont have collider in overworld
  * Milkcat init dialogue double space?
  * Richard "it all comes spilling out of me" with !
  * Milo re-ask butterfly should give shortened answer
  * Maybe put "paw prints" en rute to secret hideout
  * Onyxia missing idle anim
    * Also in credits
  * Charlie is girl, so change gender in Ony text
    * Onyxia says I nudged Charlie, before I did. Never got the name known flag
  * Charlie text also immediately to "picture yet"
  * Maybe make Florist "how are you doing" go away after 1 talk
  * Can't talk to Luna about colors at first?
  * Maybe do "Last night?" dialogue option for Luna too (in "midnight rose dialogue")
  * Luna should have different dialogue when we know Poppy is out
    * She does, but it comes AFTER luna florist moment
  * Florist moment ("if not for their colorS") s missing
  * Could have achievements for
    * Support: Find Poppy.
    * Perspective: Help Findus see the error of his ways.
  * Mayor says "university" but Findus says "University". Keep consistent.
  * Slow zoomies could use z z z
  * Achieves dont pop up / seen ingame?
  * Marri says CIT and not C.I.T.
  * Maybe disallow Butterfly dialogue with Florist after both archs are started
  * Saru findus ball toy causes exception at end
  * Maybe disallow Mayor Luna option after 1 talk
  * Check findus ball toy dialogues, sometimes it is referred to as "Ball Toy" vs "ball toy"
  * Findus and sinbad are at the Cattelatte AND CIT?
  * Findus Richard moment uses "realise" a lot
  * Manekineko juugemo first takes extra line and stops at shuringan no gurindai
    * da uses = not long -
    * Should have different dialogue after first
  * Finalemoment "taken by the Color kitnapper" when text spawns in, the html tags/bbcode for colors spawn in one at a time. It works in the end, but not when spawning.
    * Spawn in bbcode all at once. Everything inside <> including </> (this will skip some spawning, but its fine for the end)
    * Or if you are able to make the text inside spawn, while keeping the color tags that'd be awesome! A bit more fiddly to implement tho
  * After finalemoment, "esc to exit" top bar spawns. Just hide this
  * Credits length is longer than song. Fix
  * Maybe new cats should have after finale dialogue too
    * Orange, Blackcurrant (also need bigger dialoguebox)
    * Onyxia, Charlie, Mixie, Cola
  * Double check Kira dialogue
  * Does game always stutter at music swap?
  * Downright of Outer reality is very barren
    * not in there should be higher up
  * Guard behind dialogue cause error
    * But only sometimes?


Other names that can be used:
* Kuro (Blue Exorcist)
* Junji ito cats
* Bakko (kaiju 8)
* Kuraha (Noragami)
* Jiji (Kiki)
* Kyo (Fruit basket)