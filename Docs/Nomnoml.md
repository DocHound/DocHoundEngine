# Nomnoml Charts

Kava Docs supports the nomnoml UML diagram standard. For more information, see https://github.com/skanaar/nomnoml

## Simple Diagram Example

```nomnoml
[example|
  propertyA: Int
  propertyB: string
|
  methodA()
  methodB()
|
  [subA]--[subB]
  [subA]-:>[sub C]
]
```

## Simple Class Diagram

```nomnoml
[Car] -> [Driver]
[Car] -> [Engine]
[Engine] -> [Piston]
```

## Slightly Better Class Diagram

```nomnoml
[<abstract>Wagon|maxPeople:int;wheels:int|go();stop()]<:-[Car|start();stop()]
[Car]<:-[Pickup|maxCapacity:int|load()]
[Pickup]->[Trailer|wheels:int]

#fontSize: 10
#lineWidth: 1
```

## Advanced Digram Example

```nomnoml
[Pirate|eyeCount: Int|raid();pillage()|
  [beard]--[parrot]
  [beard]-:>[foul mouth]
]

[<abstract>Marauder]<:--[Pirate]
[Pirate]- 0..7[mischief]
[jollyness]->[Pirate]
[jollyness]->[rum]
[jollyness]->[singing]
[Pirate]-> *[rum|tastiness: Int|swig()]
[Pirate]->[singing]
[singing]<->[rum]

[<start>st]->[<state>plunder]
[plunder]->[<choice>more loot]
[more loot]->[st]
[more loot] no ->[<end>e]

[<actor>Sailor] - [<usecase>shiver me;timbers]

#fontSize: 10
#lineWidth: 1
```
