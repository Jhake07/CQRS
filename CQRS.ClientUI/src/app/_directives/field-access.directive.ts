import {
  Directive,
  ElementRef,
  Input,
  OnInit,
  Renderer2,
  inject,
} from '@angular/core';
import { NgControl } from '@angular/forms';
import { FormMode } from '../_enums/form-mode.enum';

@Directive({
  selector: '[appFieldAccess]',
  standalone: true,
})
export class FieldAccessDirective implements OnInit {
  @Input() formMode!: FormMode;
  @Input() editableInModes: FormMode[] = [FormMode.New];
  @Input() applyVisualCue = true;
  @Input() readonlyClass = 'bg-light';

  private readonly control = inject(NgControl);
  private readonly el = inject(ElementRef);
  private readonly renderer = inject(Renderer2);

  ngOnInit(): void {
    const formControl = this.control?.control;

    if (!formControl) return;

    const isEditable = this.editableInModes.includes(this.formMode);

    // Apply or remove form control accessibility
    isEditable ? formControl.enable() : formControl.disable();

    // Add a visual cue class (if desired and not editable)
    if (!isEditable && this.applyVisualCue) {
      this.renderer.addClass(this.el.nativeElement, this.readonlyClass);
    } else {
      this.renderer.removeClass(this.el.nativeElement, this.readonlyClass);
    }
  }
}
