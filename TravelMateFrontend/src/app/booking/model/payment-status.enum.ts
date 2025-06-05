export enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Failed = 2
}

export const PaymentStatusLabels: Record<PaymentStatus, string> = {
  [PaymentStatus.Pending]: 'Pending',
  [PaymentStatus.Completed]: 'Completed',
  [PaymentStatus.Failed]: 'Failed'
};
