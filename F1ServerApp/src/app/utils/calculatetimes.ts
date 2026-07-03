export class CalculateTimes
{
  // Seconds to string
  public static timeSecondsToString(time_secs: number): string
  {
    let output = "";
    let hours = Math.floor(time_secs / 3600);

    time_secs %= 3600;

    let minutes = Math.floor(time_secs / 60);
    let seconds = time_secs % 60;

    if (hours > 0)
    {
      output += hours.toFixed() + "h ";
    }

    output += minutes.toFixed() + "m ";
    output += seconds.toFixed() + "s";

    return output;
  }

  // Generate time output
  public static matchTimeOutput(timeInMilliseconds: number): string
  {
    let output = "";

    if (timeInMilliseconds >= 60000)
    {
      let minutes = Math.floor(timeInMilliseconds / 1000 / 60);

      output += this.padLeadingZeros(minutes, 2) + ":";

      timeInMilliseconds %= 60000;
    }

    let seconds = Math.floor(timeInMilliseconds / 1000);

    output += this.padLeadingZeros(seconds, 2) + ".";

    let milliseconds = timeInMilliseconds % 1000;

    output += this.padLeadingZeros(milliseconds, 3);

    return output;
  }

  // Generate time output
  public static matchTimeDiffOutput(timeInMilliseconds: number): string
  {
    let output = "";

    if (timeInMilliseconds > 0)
    {
      output = "+";
    }
    else if (timeInMilliseconds < 0)
    {
      output = "-";
    }

    if (timeInMilliseconds >= 60000)
    {
      let minutes = Math.floor(timeInMilliseconds / 1000 / 60);

      output += this.padLeadingZeros(minutes, 2) + ":";

      timeInMilliseconds %= 60000;
    }

    let seconds = Math.floor(timeInMilliseconds / 1000);

    output += this.padLeadingZeros(seconds, 2) + ".";

    let milliseconds = timeInMilliseconds % 1000;

    output += this.padLeadingZeros(milliseconds, 3);

    return output;
  }

  static padLeadingZeros(num: number, size: number)
  {
    let s = num + "";
    while (s.length < size) s = "0" + s;
    return s;
  }
}
