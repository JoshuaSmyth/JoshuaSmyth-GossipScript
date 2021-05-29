@GossipScript

# TODO
# Implement commments - DONE!
# Implement Variables - Global, Script, Local
# Implement Expressions
# Implement Export Page (For calling pages from another script)
# String Variable substitutions
# TODO Write Id assigner / remapper


defvar name:$bTalked_to_man	type:flag	scope:global	default:true
defvar name:$some_flag		type:flag	scope:script	default:false

# Basic Types include:
	# Integer
	# Mask (64 Values)
	# Flag (true or false)
	# String
	# Decimal <- Do I need decimals?

# Special Types Include:
	# Inventory-Items
	# Quests
	# Journal-Entries
	# Actors

@p1
{
	print text:"Some variable tests"

	# We can determine if the variable below has been declared at compile type

	# Text strings can contain expressions which the result is converted to a string
	print text:"The variable $bTalked_to_man is {$bTalked_to_man}"

    call-page node:@output

	# Don't need temps just yet
	# def-temp name:$tmpStr type:string default:""
	# set-temp name:$tmpStr value:"not" only-if:$bTalked_to_man==false
	# print text:"You have {$tmpStr} talked to the man"

	# Text strings can contain expressions which the result can be mapped
	# These are all ways of saying the same thing
	# Localisation will pull the whole string including the expression. 
	# Which means strings will need to be compiled at some stage

	# Maybe in the future
	# Compare Results
	#print text:"You have {$bTalked_to_man == false => "not"} talked to the man"
	#print text:"You have {$bTalked_to_man == false => "not" ? ""} talked to the man"
	
	# Map Results
	#print text:"You have {$bTalked_to_man => [false, true]->["not", ""]} talked to the man"
	#print text:"You have {$bTalked_to_man => [false]->["not"]} talked to the man"
	#print text:"You have {$bTalked_to_man => [false]->["not"] ? [""]} talked to the man"

	# Sets to True
	set-var name:$bTalked_to_man scope:global value:true
	
	call-page node:@output

	# Sets to false (or not true) this shows value is actually an expression
	set-var name:$bTalked_to_man scope:global value:!$bTalked_to_man

	call-page node:@output
}

[Export("g_Output")]
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