import { Directive, ElementRef, Input, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { IdentifierType } from '@app/shared/enums';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';

@Directive({
    selector: '[appPersonalIdentifier]',
})
export class PersonalIdentifierDirective implements OnInit, OnDestroy {
    @Input() identifierType!: IdentifierType;
    @Input() identifier = '';
    private subscriptions = new Subscription();
    identifierTypes: { name: string; id: string }[] = [];

    constructor(private el: ElementRef, private renderer: Renderer2, private translateService: TranslocoService) {}

    ngOnInit(): void {
        this.subscriptions.add(
            this.translateService.langChanges$.subscribe(() => {
                this.identifierTypes = [
                    {
                        name: this.translateService.translate('enums.identifierType.EGN'),
                        id: IdentifierType.EGN,
                    },
                    {
                        name: this.translateService.translate('enums.identifierType.LNCh'),
                        id: IdentifierType.LNCh,
                    },
                ];
                this.renderElement();
            })
        );
        this.renderElement();
    }

    ngOnDestroy() {
        this.subscriptions.unsubscribe();
    }

    renderElement() {
        const firstValue = this.identifierTypes.find(type => type.id === this.identifierType)?.name || '';
        const secondValue = this.identifier || '';
        const combined = `${firstValue} - ${secondValue}`;

        this.renderer.setProperty(this.el.nativeElement, 'innerHTML', combined);
    }
}
