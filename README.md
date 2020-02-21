2019-02-20

Unit3D asset importer(s) for Hexen 2 (et al?)


currently; it *just* dumps the `gzip`'ed `.pak` files into `Assets/` as a mechanism to import them

it works by mapping everything into `byte[]` which is possible only because modern PCs have more RAM

to observe copyright; no assets are included here, but, the `.meta` files are - if you have (identical copies of) the `.pak` files (and you gzip them and rename the extensions)

1. copy them into place
2. open Unity and do the full import
3. close Unity
4. do a git hard reset to kick stuff back to the commited variants
5. re-open Unity and edit stuff with consistency


.pak ; https://quakewiki.org/wiki/.pak

next? http://www.gamers.org/dEngine/quake/spec/quake-spec34/qkspec_4.htm

meembee? http://tfc.duke.free.fr/coding/mdl-specs-en.html