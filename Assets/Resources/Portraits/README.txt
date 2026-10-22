DROP YOUR CHARACTER PORTRAITS HERE
==================================

Name each image file exactly like the character's "id" in the tale JSON,
with .png, .jpg or .jpeg at the end.

Example:
    character id in JSON : "mk_meera"
    image file here      : mk_meera.png

Matching is case-insensitive; "MK_Meera.png" also works.

Recommended:
  - Square image, 512x512 or 1024x1024.
  - Face/upper body, transparent or plain background.
  - Keep the file name simple (letters, numbers, underscores).

What happens automatically:
  - Unity imports any image in this folder as a Sprite (Single) for you.
  - The game loads it at runtime and shows it in the briefing, the Persons of
    Interest panel, the victim frame in the 3D room, and the tale cards.

If a character has no image here, the game generates a coloured icon
placeholder (a unique pattern from the character id), so everything still works.

To override which id a file matches, set "portraitId" on the character in the
tale JSON. Or generate the files in the README via
"Detective Game > Validate Tale Content" in the Unity menu, which lists every
portrait that is still missing.
