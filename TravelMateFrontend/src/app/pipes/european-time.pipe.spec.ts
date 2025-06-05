import { EuropeanTimePipe } from './european-time.pipe';

describe('EuropeanTimePipe', () => {
  it('create an instance', () => {
    const pipe = new EuropeanTimePipe();
    expect(pipe).toBeTruthy();
  });
});
