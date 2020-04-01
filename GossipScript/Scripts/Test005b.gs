@GossipScript

# TODO Write Syntax plugin for VS for .gs file

@p1
{
	print text:"Some variable tests"

	# set-var name:$bTalked_to_man scope:global value:false

    call-page node:@output

	set-var name:$bTalked_to_man scope:global value:true

	call-page node:@output
}


# See if we can do an export 
@output
{

	#Should be defined in global first

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