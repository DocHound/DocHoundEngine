# Nomnoml Charts

Kava Docs supports the nomnoml UML diagram standard. For more information, see https://github.com/skanaar/nomnoml

> Note: Nomnoml chart features are automatically enabled when a nomnoml block is used in markdown. Nomnoml can also be specifically enabled using the `useDiagramsNomnoml` setting, which can be set to `true` (the default can be considered to be `auto`, although that is not a setting that can be set explicitly). Note that nomnoml can be specifically disabled by setting `useDiagramsNomnoml` to `false`.  For more information, see [Table of Contents File Structure](TOC-File-Structure)

## Simple Diagram Example

```txt
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

```txt
[Car] -> [Driver]
[Car] -> [Engine]
[Engine] -> [Piston]
```

```nomnoml
[Car] -> [Driver]
[Car] -> [Engine]
[Engine] -> [Piston]
```

## Slightly Better Class Diagram

```txt
[<abstract>Wagon|maxPeople:int;wheels:int|go();stop()]<:-[Car|start();stop()]
[Car]<:-[Pickup|maxCapacity:int|load()]
[Pickup]->[Trailer|wheels:int]

#fontSize: 10
#lineWidth: 1
```

```nomnoml
[<abstract>Wagon|maxPeople:int;wheels:int|go();stop()]<:-[Car|start();stop()]
[Car]<:-[Pickup|maxCapacity:int|load()]
[Pickup]->[Trailer|wheels:int]

#fontSize: 10
#lineWidth: 1
```

## Advanced Diagram Example

```txt
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

