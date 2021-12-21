## Allgemein:
Alle Panel in HD (horizontal) und .png

Ich kann bei Knöpfen eine bei allen UI Elementen wie Knöpfen, Inputfeldern etc. Texture hinterlegen. 
Also wäre es sinnvoll wenn du einmal ein Hintergrund für einen Knopf machst (ohne Text)
Dann können wir den einfach auf erstmal auf allen Knöpfen verwenden. 
Mach dir keine Sorgen über die größe des Knopfes mache es einfach so das es bei einem HD Bildscirm gut aussieht. Unity kann den hintergrund automatich so verzerren, das es gut aussieht.
https://docs.unity3d.com/Manual/9SliceSprites.html

Genauso brauchen wir eine Hitergrund für Inputfelder

## Start Panel:

Ist der Bildschirm den man sieht wenn man die App öffnet.

Enthält den Namen des Projekts.
Enthält unsere Namen in einer Eceke oder so... 

Von hier kann man 5 Sachen machen:
1. Einstellungen öffnen
2. Server in FirstPerson hosten
3. Server in VR hosten
4. Als Client in FirstPerson joinen
5. Als Client in VR joinen

Entscheide einfach selber wie man genau diese 5 Sachen auswählt.
Ein  Einstellungsknoft wäre sinnvoll.
Aber sonst kann man einfach 4 Knöpfe für Optionen 2 - 5 machen. 
Oder zwei Schieberegler in dem man das einstellt.   
(Bei den Schiebereglern meine ich die wie aus den Apple Einstellungen mit zwei Zuständen.)

Benötigte Datein:
1 png mit dem ganzen Panel
1 kleines png pro jeden verschiedenen Knopf
1 png mit nur dem Hintergrund

Bei Dingen wie Schiebereglern 1 png für jeden möglichen Zustand.


## Einstellungs Panel:

Dort kann man IPs und Ports festlegen die für den Verbindungsaufbau genutzt werden.

Enthält eine Liste an InputFeldern für die IPs und Ports

Inputfelder:                    (Die default Werte der Felder)
- Network IP                    (127.0.0.1)
- Network Server TCP Port       (11000)
- Network Server UDP Port       (11001)
- Networt Client TCP Port       (11002)
- Network Client UDP Port       (11003)
- Voice Signaling Server IP     (127.0.0.1)
- Voice Signaling Server Port   (11010)

Enthält einen zurück Knopf zum Start Panel


Benötigte Datein:
1 png mit dem ganzen Panel
1 kleines png pro jeden verschiedenen Knopf
1 kleines png für den Hitergrund eines leeren Inputfeldes
1 png mit nur dem Hintergrund

## First Person Pause Panel:

Dieses Panel taucht auf wenn man im First-Person Modus ECS drückt.
Dabei wird die Mause unlocked und der Spieler bewegt sich nicht mehr.

Enthält Knopf um die Konferenz zu verlassen.
Enthält ein Knopf zu fortsetzen der SpFirst Person Steurerung. 

Mann kann in diesem Panel auch neue Datein hochlanden.
Also brauchen wir eine Knopft um eine Datei zu hochlanden auszuwählen. 
Beim druck des Knopf öffnet sich ein Datei Manager PopUp welches wir schon fertig haben.
Außerdem baruchen wir einen Knopf mit dem man alle Hochgelandenen Datein von anderen Teilnehemeren anfordert.

Dazu wäre eine Liste aller Datein die hochgelanden wurden sinnvoll.
In der Liste sollten folgene Eigenscahften angezeigt werden.
- Name der Datei
- Ein Symbol ob sie local vorhanden ist oder noch runtergeladen werden muss.
- Größe der Datei

Benötigte Datein:
1 png mit dem ganzen Panel
1 kleines png pro jeden verschiedenen Knopf
1 kleines png für den Hintergrund der Liste
1 kleines png für den Hintergrund eines Eintrags in der Liste
1 png mit nur dem Hintergrund



*mit jeden verschiedenen Knopf meine ich nur wenn der Hitergrund anders ist oder du einen Knopf mit Symbol gemacht hast.