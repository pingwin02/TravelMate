import { Pipe, PipeTransform } from '@angular/core';
import { format, toZonedTime } from 'date-fns-tz';
import { enGB } from 'date-fns/locale';

@Pipe({
  name: 'europeanTime'
})
export class EuropeanTimePipe implements PipeTransform {
  transform(
    value: Date | string | number,
    timeZone: string = 'Europe/Warsaw',
    formatStr: string = 'EEEE, dd/MM/yyyy HH:mm'
  ): string {
    if (!value) return '';

    try {
      const date = new Date(value);
      const zonedDate = toZonedTime(date, timeZone);
      return format(zonedDate, formatStr, {
        timeZone,
        locale: enGB
      });
    } catch (error) {
      console.error('EuropeanTimePipe error:', error);
      return '';
    }
  }
}
