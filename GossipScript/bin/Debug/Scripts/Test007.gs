@GossipScript
{
	# TODO Implement local variables

	#TODO Allow player.money as a variable name rather than player_money
	def name:$fee			type:int	scope:global	default:200
	def name:$player_money	type:int	scope:global	default:100
}

@p1
{
	print text:"Variable Test"

	call-page node:@p2

	set-var name:$player_money value:$player_money+100

	call-page node:@p2
}

@p2
{
	[0bbf9b6d] if expr:$player_money < $fee
	{
		case-true
		{
			print text:"You do not have enough money"::[0bbf9b6d]
		}
		case-false
		{
			print text:"You have enough money"
		}
	}
}