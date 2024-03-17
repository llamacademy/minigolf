# Show a NavMesh In Game

In this tutorial repository and [accompanying video](https://youtu.be/zxmz-LV6E6g) you will learn how you can generate a Mesh at runtime based on the [NavMesh Triangulation](https://docs.unity3d.com/ScriptReference/AI.NavMesh.CalculateTriangulation.html).

This can be used for debugging purposes or some cool ground-based shader effects. You will learn how to show the Navigation Mesh per agent type with the NavMeshSurface component from the [Navigation Components](https://docs.unity3d.com/Packages/com.unity.ai.navigation@1.1/manual/index.html) as well as show different materials per area.

As of the Navigation Components 1.1.3, `NavMesh.CalculateTriangulation()` does not properly generate `NavMeshTriangulation.areas` per Navigation Area. Showing navigation areas therefore **only works if you build using the legacy system**, which does not support different navigation meshes per NavMeshAgent type.

[![Youtube Tutorial](./Video%20Screenshot.jpg)](https://youtu.be/zxmz-LV6E6g)

## Supporters
Have you been getting value out of these tutorials? Do you believe in LlamAcademy's mission of helping everyone make their game dev dream become a reality? Consider becoming a Patreon supporter and get your name added to this list, as well as other cool perks.
Head over to the [LlamAcademy Patreon Page](https://patreon.com/llamacademy) or join as a [YouTube Member](https://www.youtube.com/channel/UCnWm6pMD38R1E2vCAByGb6w/join) to show your support.

### Phenomenal Supporter Tier
* Andrew Bowen
* YOUR NAME HERE!

### Tremendous Supporter Tier
* Bruno Bozic
* YOUR NAME HERE!

### Awesome Supporter Tier
* AudemKay
* Matt Parkin
* Ivan
* Reulan
* Iffy Obelus
* Dwarf
* YOUR NAME HERE!

### Supporters
* Trey Briggs
* Matt Sponholz
* Dr Bash
* Tarik
* Sean
* ag10g
* Elijah Singer
* Lurking Ninja
* Josh Meyer
* Ewald Schulte
* Dom C
* Andrew Allbright
* AudemKay
* EJ
* Claduiu Barsan-Pipu
* Sschmeil22
* Ben
* YOUR NAME HERE!

## Other Projects
Interested in other Topics in Unity? 

* [Check out the LlamAcademy YouTube Channel](https://youtube.com/c/LlamAcademy)!
* [Check out the LlamAcademy GitHub for more projects](https://github.com/llamacademy)

## Socials
* [YouTube](https://youtube.com/c/LlamAcademy)
* [Facebook](https://facebook.com/LlamAcademyOfficial)
* [TikTok](https://www.tiktok.com/@llamacademy)
* [Twitter](https://twitter.com/TheLlamAcademy)
* [Instagram](https://www.instagram.com/llamacademy/)
* [Reddit](https://www.reddit.com/user/LlamAcademyOfficial)

## Requirements
* Requires Unity 2021.3 LTS or higher.
* Universal Render Pipeline