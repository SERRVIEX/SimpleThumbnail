# SimpleThumbnail

![Version](https://img.shields.io/badge/Version-v1.0.0-brightgreen.svg)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/SERRVIEX/SimpleRecyclerCollection/blob/main/LICENSE) 
[![Contact](https://img.shields.io/badge/LinkedIn-blue.svg?logo=LinkedIn)](https://www.linkedin.com/in/sergiu-ciornii-466395220/)

## Requirements
[![Unity 2020+](https://img.shields.io/badge/unity-2020+-black.svg?style=flat&logo=unity&cacheSeconds=2592000)](https://unity3d.com/get-unity/download/archive)
![.NET 4.x Scripting Runtime](https://img.shields.io/badge/.NET-4.x-blueviolet.svg?style=flat&cacheSeconds=2592000)

## Description
Script for taking screenshots in Unity.

## How to use?
There are two ways to use this script.
1. First, designed for ```Editor``` only, the script needs to be attached to an GameObject on the scene, connect the ```Camera``` from which the pixels will be read and start the game. And when you press the ```P``` key (or the one you specified), a screenshot will be taken and the file path will be logged in the console.
2. The second way is to run a static coroutine from outside the script.
```csharp
StartCoroutine(SimpleThumbnail.Take(Camera, Action<Texture2D>, float))
```

## Additional
It is also possible to focus the ```Camera``` on an object with a ```Renderer``` component. To do this, you need to call the static method ```Focus```. This should be done when you need to make a preview for a certain 3D object.
```csharp
SimpleThumbnail.Focus(Camera, GameObject, float))
```

A coroutine is also available to remove transparent pixels around the edges.
```csharp
StartCoroutine(SimpleThumbnail.Crop(Texture2D, bool, Action<Texture2D>))
```

## License
MIT License

Copyright (c) 2023 SERRVIEX

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.