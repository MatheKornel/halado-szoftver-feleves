## Program célja
Haladó szoftverfejlesztés tárgy keretein belül egy konzolos rétegzett CRUD alkalmazás készítése.

## Feladat leírása
Az A PVM Kft felbérelte Önt, a HSZF szoftverfejlesztő cég egyik alkalmazottját, hogy segítsen feldolgozni az elmúlt időszak vízállását.

Adott egy JSON formátum, amelyben az egyes mérési pontok és az ahhoz tartozó mérési adatok találhatóak. A program képes legyen a felhasználótól egy ilyen formátumú fájlt betölteni és a benne lévő adatokat elmenteni egy adatbázisba. Az ehhez szükséges adatbázis sémát, valamint az egyes táblák létrehozása is az Ön feladata.

További követelmény ha egy újabb fájl betöltésekor, ha már létezik az adott mérőpont, akkor csak az új méréseket adjuk hozzá!

Legyen mód mérőpontot létrehozni, módosítani, törölni a program futása alatt.
Arra is legyen mód, hogy kézzel is új utakat tudjunk hozzáadni. Ha az adott mérőponton a vízállás magasabb, mint az adott mérőponthoz tartozó bármelyik másik erről a tényről esemény formájában értesítsük a felhasználót.

Generáljon statisztikát a következőkről és mentse el egy fájlba:

- Mérőpontonként az adott mérőponthoz tartozó mérések számát.
- Mérőpontonként az átlagos vízállás mértékét, a legmagasabb és legalacsonyabb mérések adataival
- Mérőpontonként azokat a méréseket, ahol a csapadék mennyisége több mint az átlag..

Opcionálisan legyen lehetőség egy elérési útvonalat megadni ahova a kimeneti fájl elmentésre kerül.

Legyen lehetőség listázni az egyes mérőpontokat, de arra is legyen mód, hogy keressünk, illetve szűkítsük a lista eredményét. Ehhez a funkcionalitáshoz hozzon létre egy alap keresési funkciót, amely az egyes tulajdonságok alapján képes keresni. Előfordulhat az, hogy a felhasználó csak egy bizonyos tulajdonság alapján akar keresni, de lehet az összeset megadja, ezt kezelje a program!

Manuális teszteléshez készítsen 2-3 bemeneti fájlt. 
A Json fájlt igény szerint bővítse.  

+ 5% Ha a témához köthető, legalább 3 easter egg elrejtése az alkalmazáshoz. (Mérőpont név, vízállás érték, stb…) 

+ 5% ha leadási határidő előtt leadja és sikeresen megvédi a félévesét a hallgató.

## Használt technológiák
- Eseménykezelés
- Fájl és könyvtárkezelés
- LINQ
- XML és JSON fájlkezelés
- DLL kezelés
- Reflexió
- Adatbázis készítés
- Rétegződés
- Unit teszt

## Mappa struktúra
```
halado-szoftver-feleves/
├── JY24WV_HSZF_2024251.Application/
│ └── DataService.cs
│ └── PointService.cs
├── JY24WV_HSZF_2024251.Console/
│ └── data1.json
| └── data2.json
| └── data3.json
| └── Program.cs
├── JY24WV_HSZF_2024251.Model/
| └── MeasurementData.cs
| └── MeasurementPoint.cs
| └── MeasurementRoot.cs
├── JY24WV_HSZF_2024251.Persistence.MsSql/
| └── AppDbContext.cs
| └── MeasurementDataProvider.cs
| └── MeasurementPointProvider.cs
├── JY24WV_HSZF_2024251.Test/
| └── DataServiceTester.cs
| └── PointServiceTester.cs
├── JY24WV_HSZF_2024251.sln
├── .gitignore
├── README.md
```
