export enum CronPeriods {
    Weekly = '0 0 5 ? * MON *',
    Monthly = '0 0 5 1 1/1 ? *',
    Yearly = '0 0 5 1 1 ? *',
}
