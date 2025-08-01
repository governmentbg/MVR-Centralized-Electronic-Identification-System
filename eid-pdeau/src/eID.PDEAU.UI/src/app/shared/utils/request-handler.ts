import { Observable, Subscription, delay, first, of, switchMap, tap } from 'rxjs';
import { ZodError } from 'zod';

export type RequestHandlerStatus = 'idle' | 'loading' | 'success' | 'error';
const DEFAULT_RETRY_ATTEMPTS = 3;

export class RequestHandler<T, RequestParameters extends Parameters<any>> {
    private subscription: Subscription | null = null;
    isLoading = false;
    error = null;
    status: RequestHandlerStatus = 'idle';
    data: T | null = null;

    constructor(
        private options: {
            requestFunction: (...args: RequestParameters) => Observable<T>;
            onSuccess?: (data: T, args: RequestParameters) => void;
            onError?: (error: any) => void;
            onInit?: () => void;
            onComplete?: () => void;

            // retryAttempts?: number;
            // runOnInit?: boolean;
        }
    ) {}

    execute(...args: RequestParameters) {
        // const retryAttempts =
        //     typeof this.options.retryAttempts === 'number' ? this.options.retryAttempts : DEFAULT_RETRY_ATTEMPTS;

        this.options.onInit?.();

        this.subscription = of(null)
            // The first pipe is added to overcome ExpressionChangedAfterItHasBeenCheckedError exception
            // by moving the setting of RequestHandler's properties in the next cycle rxjs way.
            .pipe(
                delay(0),
                tap(() => {
                    this.isLoading = true;
                    this.status = 'loading';
                }),
                switchMap(() => this.options.requestFunction(...args))
            )
            .pipe(first())
            // .pipe(retry(retryAttempts), first())
            .subscribe({
                next: data => {
                    this.isLoading = false;

                    this.data = data;
                    this.status = 'success';
                    this.options.onSuccess?.(data, args);
                },
                error: error => {
                    this.error = error;
                    this.status = 'error';
                    this.isLoading = false;
                    this.options.onError?.(error);

                    // Should we rethrow all errors or just handle it locally,
                    // or it could be an option to pass a flag to bubble the error for a global error handler
                    // will there be a global catch for logging purposes?
                    // For now I will leave just a console.error to see in the console what validations are failing
                    if (error instanceof ZodError) {
                        console.error(`Validation error:`, error.format());
                    }
                },
                complete: () => {
                    this.options?.onComplete?.();
                },
            });

        return this;
    }

    unsubscribe() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
}
