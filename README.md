This is a small [Vintage Story](https://www.vintagestory.at) library to allow to patch patches. With this installed, a content mod can add a file of blockpatch patches at the asset location `config/blockpatchpatches.json`. Patches target all blockpatches, either in `worldgen/blockpatches.json` or in the `worldgen/blockpatches` folder.

The allowed format resembles very closely the blockpatch one, as seen in the example patch:
```json
{
  "comment": "Patch berries",
  "code": "@.*(berry).*",
  "op": "replace",
  "minTemp": -100,
  "maxTemp": 100,
  "minRain": 0.0,
  "maxRain": 1.0,
  "minFertility": 0.0,
  "maxFertility": 1.0,
  "chance": 2000.0,
  "minForest": 0.0,
  "maxForest": 1.0,
  "minY": 0.0,
  "maxY": 1.0,
  "quantity": { "dist": "invexp", "avg": 80.0, "var": 30.0 }
}
```
All numerical values of a block patch can be patched using one of three operation types: `add`, `multiply`, `replace`. The first two will use the existing value dynamically, multiplying it or adding the value in the patch to it. `replace` will instead simply replace the value.

Currently supported value types are:
- Numerical values (`int`, `float`)
- `NatFloat` (as an example, see "quantity" in the patch above)
- Strings and lists of strings
  
Only numerical values support multiplication, string support addition and replacement, while the rest only support replacement. You can make multiple patches targeting the same code to get around this.
