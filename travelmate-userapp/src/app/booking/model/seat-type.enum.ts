export enum SeatType {
  Economy = 0,
  Business = 1,
  FirstClass = 2,
}

export const SeatTypeLabels: Record<SeatType, string> = {
  [SeatType.Economy]: 'Economy',
  [SeatType.Business]: 'Business',
  [SeatType.FirstClass]: 'FirstClass',
};
