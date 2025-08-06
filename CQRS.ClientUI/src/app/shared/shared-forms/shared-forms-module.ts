import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FieldAccessDirective } from '../../_directives/field-access.directive';
import { StatusClassPipe } from '../../_pipes/status-class-pipe';
import { Paginator } from '../paginator/paginator';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FieldAccessDirective,
    StatusClassPipe,
    Paginator,
    ReactiveFormsModule,
  ],
  exports: [
    ReactiveFormsModule,
    FieldAccessDirective,
    StatusClassPipe,
    Paginator,
  ],
})
export class SharedFormsModule {}
