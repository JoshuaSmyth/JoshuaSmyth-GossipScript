﻿[00aabbccdd] @GossipScript

# This is a comment

@p1
{
	say actor:"narrator" text:"1. Hello World"
	#debug message:"Calling Page 2"
	call-page node:@p2
	#debug message:"Returning from Page 2"
	say actor:"narrator" text:"5. Last"
	say actor:"narrator" text:"6. KthxBye!"
}

@p2
{
	say actor:"narrator" text:"2. Hello from page 2"
	call-page node:@p3
}

@p3
{
	say actor:"narrator" text:"3. Hello from page 3"
	say actor:"narrator" text:"4. We are going to return now"
	return
}