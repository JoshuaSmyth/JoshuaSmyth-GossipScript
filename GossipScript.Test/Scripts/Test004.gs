@GossipScript

@p1
{
	print text:"Following Total Wait Time should be approx 1 second"
	parallel
	{
		block
		{
			print text:"Begin Block1"
			wait time:1000
			print text:"End Block1"
		}
		block
		{
			print text:"Begin Block2"
			wait time:1000
			parallel
			{
				block
				{
					print text:"Begin Block2a"
					wait time:1000
					print text:"End Block2a"
				}
				block
				{
					print text:"Begin Block2b"
					wait time:1000
					print text:"End Block 2b"
				}
			}
			print text:"End Block2"
		}
		block
		{
			print text:"Begin Block3"
			wait time:1000
			print text:"End Block3"
		}
	}
	print text:"Done!"
}