[74065e00] @GossipScript
{

}

# TODO Write Id assigner / remapper

[74065e00] @p1
{
	[2a6b95e0] say actor:"narrator" text:"1. Hello my friend"::[0f7ed73b]
	[0f3b0f3b] debug message:"Calling Page 2"
	[0bbf9b6d] call-page node:@p2
	[add3ed7d] debug message:"Returning from Page 2"
	[aeeeccac] say actor:"narrator" text:"5. Last"::[d7ed7bf0]
	[0f3ed7e0] say actor:"narrator" text:"6. KthxBye!"::[f33ed7bf]
}

[0bbf9b6d] @p2
{
	[add3ed7d] say actor:"narrator" text:"2. Hello from page 2"::[d7ed7bf0]
	[0f3b0f3b] call-page node:@p3
}

[74065e00] @p3
{
	[0f3b0f3b] say actor:"narrator" text:"3. Hell from page 3"::[d76d73f0]
	[add3ed7d] say actor:"narrator" text:"4. We are going to return now"::[d1ed2bf8]
	[0f3b0f3b] return
	[add3ed7d] error message:"We should never get here""::[d7ea4bf2]
}