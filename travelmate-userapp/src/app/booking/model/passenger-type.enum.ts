export enum PassengerType {
  Adult = 0,
  Child = 1,
  Baby = 2
}

export const PassengerTypeLabels: Record<PassengerType, string> = {
  [PassengerType.Adult]: 'Adult',
  [PassengerType.Child]: 'Child',
  [PassengerType.Baby]: 'Baby'
};
