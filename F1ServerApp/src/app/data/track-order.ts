// Single source of truth for the per-year track calendar order, shared by
// ChampionshipsComponent (sortTracks) and CreateChampionshipComponent (adjustTracks).
// Values are track numbers (ChampionshipTrackViewData.trackNumber / TrackData.index).
const TRACK_ORDER_BY_YEAR: ReadonlyMap<number, readonly number[]> = new Map<number, readonly number[]>(
[
  [ 2020, [ 0, 3, 21, 2, 22, 4, 5, 20, 6, 1, 17, 7, 9, 10, 11, 12, 18, 13, 15, 19, 16, 14 ] ],
  [ 2021, [ 3, 23, 24, 4, 5, 20, 1, 17, 29, 7, 9, 10, 22, 18, 15, 19, 16, 28, 25, 14 ] ], // Turkey is between Sochi and Texas
  [ 2022, [ 3, 25, 0, 23, 26, 4, 5, 20, 6, 7, 17, 9, 10, 22, 11, 12, 13, 15, 19, 16, 14 ] ],
  [ 2023, [ 3, 25, 0, 20, 26, 23, 5, 4, 6, 17, 7, 9, 10, 22, 11, 12, 13, 28, 15, 19, 16, 27, 14 ] ],
  [ 2024, [ 3, 25, 0, 13, 2, 26, 23, 5, 6, 4, 17, 7, 9, 10, 22, 11, 20, 12, 15, 19, 16, 27, 28, 14 ] ],
  [ 2025, [ 0, 2, 13, 3, 25, 26, 23, 5, 4, 6, 17, 7, 10, 9, 22, 11, 20, 12, 15, 19, 16, 27, 28, 14 ] ],
  [ 2026, [ 0, 2, 13, 3, 25, 26, 6, 5, 4, 17, 7, 10, 9, 22, 11, 30, 20, 12, 15, 19, 16, 27, 28, 14 ] ]
]);

/**
 * Returns the track calendar order (track numbers, in race order) for a given F1 game year.
 * Returns an empty array when no order is defined for the year.
 */
export function getTrackOrderForYear(gameVersionYear: number): readonly number[]
{
  return TRACK_ORDER_BY_YEAR.get(gameVersionYear) ?? [];
}

/**
 * Extracts the numeric game year from a game version string, e.g. "F1 2020" -> 2020.
 * Returns undefined when no year can be parsed from the given string.
 */
export function parseGameVersionYear(gameVersion: string): number | undefined
{
  const match = gameVersion.match(/(\d{4})/);

  return match ? Number(match[1]) : undefined;
}
