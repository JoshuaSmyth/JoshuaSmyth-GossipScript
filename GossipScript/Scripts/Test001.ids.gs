@GossipScript::[0]

# TODO Write Id assigner / remapper

@p1::[1]
{
	[a] say actor:"narrator" text:"1. Hello my friend"::[2]
	[b] debug message:"Calling Page 2"
	[c] call-page node:@p2
	[d] debug message:"Returning from Page 2"
	[e] say actor:"narrator" text:"5. Last"::[3]
	[f] say actor:"narrator" text:"6. KthxBye!"::[4]
}

@p2::[5]
{
	[g] say actor:"narrator" text:"2. Hello from page 2"::[6]
	[h] call-page node:@p3
}

@p3::[7]
{
	[i] say actor:"narrator" text:"3. Hell from page 3"::[8]
	[j] say actor:"narrator" text:"4. We are going to return now"::[9]
	[k] return
	[l] error message:"We should never get here""::[10]
}

@ids
{
	[0]::2a6b95e0-4469-4c44-ad42-296d9246510e
	[1]::2e5c7c86-d891-4764-b851-efcbb93efa84
	[2]::aced85cf-4ca5-4399-9869-6e98700915c7
	[3]::56302111-7b3d-4dd4-a620-d33ba008d79e
	[4]::0f3bfc97-7406-49a0-ac24-91dcb5a0d7e0
	[5]::0bbf9b6d-8f81-46c2-a941-22ff2ce24aa3
	[6]::1bcfbe0e-9128-4338-84b0-caff0fcdd03a
	[7]::6f635a3f-4761-4082-a638-a54af5a44031
	[8]::e5ee7431-060f-4220-aefa-b858d2f554c5
	[9]::89e80d17-6882-4454-bb34-1a849cd00328
	[10]::f6413c64-0520-4190-b920-8700d4a6fa1e
	[a]::
}