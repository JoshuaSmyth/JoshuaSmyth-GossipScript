@GossipScript
{
	def name:$bTalked_to_man type:flag	scope:global	default:false
}

@p1
{
	print text:"Some variable tests"

    call-page node:@output

	set-var name:$bTalked_to_man scope:global value:true

	call-page node:@output
}

@output
{
	if expr:$bTalked_to_man==true
	{
		case-true
		{
			print text:"You have talked to the man"
			
		}
		case-false
		{
			print text:"You have not talked to the man"
		}
	}
}