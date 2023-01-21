## BloodthirstCore
Is my personal repo containing my variaty of tools/editors/systems that i use as a backbone to serious projects.
Ill gtry to briefly present most of the systems that the package offers.

## How the package is divided
### The package is divided mainly into 5 parts (as seen from the folder structure)
* Common : Utils and Helper extensions that provide handy methods/shortcuts , this is used all over the package.
* Runtime : Systems that are meant to be used during runtime/playmore.
* Editor : EditorWindows , CodeGenerators , SceneViewGui , Unity hooks and workflow tool , stuff to help you make stuff.
* Graphics : Contains interresting shaders and graphics related stuff (not much going on for it for now)
* Tests : UnitTests for the most important and delicate systems to make sure everthing is in check after we make changes.

### Note about dependencies/settings
This repo needs a few dependencies/settings :
- API compatibility level needs to be set to .NET framework (and not .NET Standard 2.1)
- Odin inspector (Optional)
- DOTween (You need to remove/add "DOTWEEN" to your Scripting Define Symbols depending on the package being present of not )
- TextMeshPro

### Cool ! Now that you have a brief a idea about what this package will provide ill try to introduce the most interresting systems along with a brief description to get you familiar.

### Runtime
* AdvancedPool : The runtime part of an utomatic pooling system , all you have to do is mark a prefab in the prject with a certain component and "refresh" and TADA ! 
The system will generate a strongly typed pool for all marked prefabs , put it in a special scene made for pools and do all the tedious heavy lifting.
* CommandSystem : A robust customizable to systems that manages async tasks/action in multiple-flexible ways : Task-Queue, Task-List , Task-Tree ... The backbone of every serious project.
* BProvider : a custom Dependecy injection framework
* BJson : a custom JSON serializer that works with unity objects.
* BHotReload : "True" hot reloading (yes that's right) , edit the code while in playmode and keep your data after the recompilation.
* BNodeTree : The runtime part of a custom generic NodeEditor , you need to structure data in a tree structure while having editor support ? say no more ! inherite the class "NodeBase" and start editing.
* BDeepCopy : A lightweight performant library that helps make deep copies of ANY C# object with a few customization options.
* BEventSystem : a lightweight GameEventSystem to reduce coupling between systems messaging eachother.
* BExtensions : Utils and extensions mainly for runtime scenarios.
* BISD System : Stands for "Behaviour-Instance-State-Data) , a way i commonly used to separate code/concerns , think of it as a Model-View-Component but in a more unity way.
* BRecorder : The runtime part of an in-game command recording system , think of it as a replay system but for debugging purposes.
* BType : Contains extra type-reflection related features that are used by other systems.
* SocketLayer : My first jab at a custom nextwork layer , supports UDP-TCP.
* WindowSystem : An elegant way to manage UI dialogs and window seemlessly with stacking and transitions.
### EditorTools
* BInspector : a generic custom inspector (WIP)
* BNodeTree : generic Node tree editor
* BExcel : a small excel file reader/editor
* FindShader : Helps you locate materials that use the same shader.
* BSearch : Looking for an THAT specif asset that has a string field "MyFavoriteNPC" but you forgot where it was ? this tool got you covered ! this tool allows for searching for a specific value/ref/asset that was used/mentioned/referenced in the entire project (supports custom filters)
* AdvancedTypeCache : Using Roslyn compiler to see which file contains which type in a thorough way.
* BAdapter : the glue between systems and the custom Dependency injection library (BProvider)
* SceneCreator : A tool for generating scenes and referencing them in a clean way.

## Lastly
That wasn't a full description of all the systems available , if you like what you see feel free to dig through the code or you it.

This repo is under the 	__Do What The F*ck You Want To Public License WTFPL__ (it's a real license , google it)
