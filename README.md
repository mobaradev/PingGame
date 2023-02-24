# Ping Game

![PingGame menu](https://i.imgur.com/rzqZo7I.png)

The game was created as a challenge for the cybersecurity contest "Capture the flag" organized by PING cybersecurity club in December 2022.

The concept of the game is a simple table tenis single-player gameplay with online ranking of scores. The game uses diegetic ui design (all necessary information such as number of points and number of lives are displayed in the game world).

![PingGame gameplay](https://i.imgur.com/IYIgXng.png)

The code is written in C# with Unity3D engine (version 2022.1.19f1).

**Note:** using different version of Unity3D should work, however if you want to play with online server, the results may not be validated correctly. The server-side by default verifies version of Unity3D from user-agent that is sent from UnityWebRequest, so different version than 2022.1.19 is considered as suspicious for the server and thus that request will be rejected (as a potential cheater's request).

## Try it
### Game (client):
https://github.com/mobaradev/PingGame/releases/tag/v1.1

Check releases to download the game client. It is available for:
* [Windows](https://github.com/mobaradev/PingGame/releases/download/v1.1/PingGame_v1_1_Windows.zip)
* [MacOS](https://github.com/mobaradev/PingGame/releases/download/v1.1/PingGame_v1_1_Mac.zip)
* [Linux](https://github.com/mobaradev/PingGame/releases/download/v1.1/PingGame_v1_1_Linux.zip)

### Server:
Ping server with rankings and scores is not online anymore, since the PingCTF competition has ended.
However, you can host your own server on your localhost using the PingGame Server that is available here:
https://github.com/mobaradev/PingGame-server



## Cheating score prevention
### Custom Random Class
Custom random class was implemented, the class requires a seed in constructor.
The same seed will always return the same string of values

*for example:*
```csharp
RandomNumbers rn = new RandomNumbers("abc"); // seed: "abc"
```
|Function call|Value  |
|--|--|
| rn.GetNumber(0, 100) | 5 |
| rn.GetNumber(0, 100) | 14 |

Calling this function is used in certain game elements to make falsification of the score harder. Read more about it in the next section.

In version 1.1, the custom RandomNumbers class was replaced with more polished [UnifiedRandom](https://github.com/mobaradev/UnifiedRandom) class.

### Variables used in score verification

When player hits or misses a ball, the following code is executed:
```csharp
this._gameManager.points += 1 * this.ballId;
this._gameManager.ballsHit.Add(this.ballId);

int pts = (int)Math.Round(this.transform.localScale.x * 10) + (int)Math.Round(this.transform.localScale.y * 20) + (int)Math.Round(this.transform.localScale.z * 30);

if (this.ballId == 1) pts += 34;
else if (this.ballId == 2) pts += -11;
else if (this.ballId == 3) pts += 2;
else if (this.ballId == 4) pts += 5;
else if (this.ballId == 5) pts += 6;

List<int> pList = new List<int>();
for (int i = 0; i < 5; i++)
{
	pList.Add(this._gameManager.r1.GetNumber(1, 301));
}

pts += pList[this.ballId - 1];

GameObject.FindGameObjectWithTag("Verifier").transform.Translate(0, (pts), 0);
```

The pts variable is initialized with taking into account ball's x, y and z scales. They are always the same (unless cheated in runtime), so that initial value can be considered as constant.
```csharp
int pts = (int)Math.Round(this.transform.localScale.x * 10) + (int)Math.Round(this.transform.localScale.y * 20) + (int)Math.Round(this.transform.localScale.z * 30);
```
Depending on ball type, the constant number of points is added to pts variable.
For example for the blue ball (id = 2):
```csharp
pts += -11;
```

Then, the list of integers called **pList** is created, and is filled with 5 pseudo-random numbers (1 to 301) from **UnifiedRandom/RandomNumbers** class.

```csharp
pList.Add(this._gameManager.r1.GetNumber(1, 301));
```
and only one value from that list is actually used in **pts** variable:
```csharp
pts += pList[this.ballId - 1];
```

However, other 4 calls are necessary. Each GetNumber() call gets next pseudo-random number from **UnifiedRandom/RandomNumbers** class and this calls are made deliberately, to make it harder to predict the numbers and cheat the score.

As a final line of ballHit / ballMissed event, we move the on-scene object **"Verifier"** by distance of pts variable:
```csharp
GameObject.FindGameObjectWithTag("Verifier").transform.Translate(0, (pts), 0);
```

### Secret Code
When the game ends (player missed over 12 balls), the secret code is prepared. It takes position of the **"Verifier"** game object, that was changed every time one ball was hit or missed with the participation of pseudo-random numbers from **UnifiedRandom/RandomNumbers**.

```csharp
public string PrepareSecretCode()
{
	UnifiedRandom rn = new UnifiedRandom(this.gameObject.tag);

	int x = (int)((GameObject.FindGameObjectWithTag("Verifier").transform.position.y * rn.GetNumber(0, 500)) +

	GameObject.FindGameObjectWithTag("Verifier").transform.position.z + rn.GetNumber(2, 250));

	string text = this.points + "-" + (double)(x - rn.GetNumber(0, x + (int)GameObject.FindGameObjectWithTag("Verifier").transform.position.x));
	text = HashString(text);

	return text;
}
```

Note that UnifiedRandom takes GameManager object's tag as a seed. It's harder to access that value, since it's accessed during runtime and can't be seen by simple decompilation of the C# code.

The **x**  and random part of the secret code is then put with declared number of points from points variable.
```csharp
string text = this.points + "-" + (double)(x - rn.GetNumber(0, x + (int)GameObject.FindGameObjectWithTag("Verifier").transform.position.x));
```

**Points** variable can be easily changed during runtime using for example CheatEngine, however the other components of Secret Code will not match.

The whole string is then hashed and returned in that form to make it even harder to understand meaning and components of that value.

### Sending score to the server
'/send_score' POST request requires 6 parameters:  
  
| Name         | Data type | Description                             |  
|--------------|-----------|-----------------------------------------|  
| points       | number    | number of points declared by player     |  
| nick         | string    | player's nick                           |  
| jerseyNumber | number    | player's jersey number (symbolic value) |  
| list         | string    | list of ids of balls hit                |  
| list2        | string    | list of ids of balls missed             |  
| ss           | string    | hash of secret code

It is handled in GameManager's IEnumerator Upload():

```csharp
WWWForm form = new WWWForm();
form.AddField("points", this.points);
form.AddField("nick", nick);
form.AddField("jerseyNumber", jerseyNumber);
form.AddField("list", String.Join("", this.ballsHit.ToArray()));
form.AddField("list2", String.Join("", this.ballsMissed.ToArray()));

string s = this.PrepareSecretCode();
string ss = "";
for (int i = 0; i < s.Length; i++)
{
	ss = ss + s[i] + this.rSeeds[1][i] + this.rSeeds[0][i];
}
form.AddField("ss", ss);

using (UnityWebRequest www = UnityWebRequest.Post(this._serverUrl + "/send_score", form))
...
```

If server returns the flag, the game will change the scene to show the flag.

That scene is always located in game's files but without response from server is useless because it does not contain the flag string - the string is displayed from server's response.

If the result is validated correctly but the number of points is not enough to obtain a flag - the game will show green message saying that the result was saved.
Otherwise, the red message will appear that can mean either connection issues or that your game's score was not validated by the server side.


## Runtime cheating prevention
There is a function name _gX() that keeps track of properties of crucial game objects:

 - Barrier
 - Racket

Any change during runtime of barrier's position, rotation and size of barrier or racket's scale will result in a different value returned by that function, which will be caught by GameManager's Update function:

```csharp
string nx = this._gX();
this._x.Add(nx);
this._x = this._x.Distinct().ToList();

if (this._x.Count > 1)
{
	int z = this.r1.GetNumber(1, 100);
	this._x.Add(z.ToString());
	this._x.Clear();
}
```
If number of values generated by _gX function is larger than 1, then it means that at least one of Barrier's or Racket's components was changed (cheated). Update method will call **UnifiedRandom/RandomNumbers** GetNumber() function once, to generate an additional, unexpected value and thus destroy the valid (expected by server) set of numbers used to generate the Secret Code.


## Methods to cheat the game and bypass anticheat
There are 2 main ways to get a flag (cheat the game):
* Prepare a request with all correct 6 parameters and proper user-agent to fool the server
* Use runtime methods or mod the game to get points easily and at the same time not cause changes to the secret code and other anticheat elements

In PING CTF 2022, over a dozen users cheated the game, and from information provided by participants all of them used the second method. At least few users tried the first way but none of them succeed.

It is of course possible to do so but this way was much more complicated and harder to understand, especially in that short time.

**Note** that server-side code was not revealed until the end of the contest.

## Write-ups from the cheating-side
Here are some interesting write-ups made by PING CTF 2022 participants.

https://n0x.cc/posts/3-pingctf-2022-pinggame/

by N0x

https://github.com/suvoni/CTF_Writeups/tree/main/pingCTF_2022

by [suvoni](https://github.com/suvoni)


## Version  
-   Version 1.1 comes with no default server set (since PING CTF has ended).  
    Feel free to use your own server and set in in the game's settings.
-   Menu bugs of 'F' and 'Space' key events wrongly interpreted fixed.
- Script source files locations were rearranged
- UnifiedRandom class replaced the previous RandomNumbers class. It works in almost the same way, however it's not compatible with old server version 1.0. Make sure to use the same client and server's version.
  
## Author
  
Michal Obara (@mobaradev)  
[mobaradev@yahoo.com](mailto:mobaradev@yahoo.com)  
http://www.mobaradev.com

3D Racket model - [Jasmine Wells](https://github.com/5006845293)
  
## License  
  
The MIT License  
  
Copyright 2023 Michal Obara (mobaradev)  
  
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:  
  
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.  
  
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
