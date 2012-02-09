
class TimeStamp
{
	var day : int;
	var month : int;
	var year : int;
	
	var hour : int;
	var minute : int;
	var second : int;
	
	function TimeStamp(timeStr : String)
	{
		// EXAMPLE:  "4/19/2010 12:00:00 AM"
		
		var segments : String[] = timeStr.Split(" "[0]);
		
		var dateSeg : String[] = segments[0].Split("/"[0]);
		
		month = parseInt(dateSeg[0].Substring(1));
		day = parseInt(dateSeg[1]);
		year = parseInt(dateSeg[2]);
		
		
		var timeSeg : String[] = segments[1].Split(":"[0]);
		
		hour = parseInt(timeSeg[0]);
		
		if(segments[2] == "PM")
		{
			hour += 12;
		}
		else if(hour == 12) // Not PM, but the hour is 12, therefore midnight AKA 0.
		{
			hour = 0;
		}
		
		minute = parseInt(timeSeg[1]);
		second = parseInt(timeSeg[2]);
		
	}
	
	
	function GetMinuteSpan(otherTS : TimeStamp) : int
	{
		var hourDif : int = otherTS.hour - hour;
		var minuteDif : int = hourDif * 60;
		minuteDif += (otherTS.minute - minute);
		return minuteDif;
	}
	
	function TotalMinutes() : int
	{
		return minute + hour*60;
	}
	
}