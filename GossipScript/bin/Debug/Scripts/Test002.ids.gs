[00aabbcc] @GossipScript
{

}

@p1
{
	once-only
	{
		case-true
		{
			say actor:"Narrator" text:"This text should appear on the first run only."
		}
		case-false
		{
			say actor:"Narrator" text:"This text should ONLY appear on subsequent runs."
		}
	}

	say actor:"Narrator" text:"- Select Option -"
	show-options
	{
		option text:"Option A"
		{
			say actor:"Narrator" text:"You selected A"
		}
		option text:"Option B"
		{
			say actor:"Narrator" text:"You selected B"

			show-options remove-on-select:true
			{
				option text:"Option 1"
				{
					say actor:"Narrator" text:"You selected 1"
				}
				option text:"Option 2"
				{
					say actor:"Narrator" text:"You selected 2"
				}
			}
		}
		option text:"Option C" exit-on-select:true
		{
			say actor:"Narrator" text:"You selected C"
		}
	}

	say actor:"Narrator" text:"KThx Bye!"
}