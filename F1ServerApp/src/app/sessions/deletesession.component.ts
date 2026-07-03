import { ChangeDetectorRef, Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { SessionViewData } from '../data/sessionviewdata';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent } from '@angular/material/dialog';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatFormField, MatHint, MatLabel, MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button'
import { MatInputModule } from '@angular/material/input';

@Component(
{
  imports: [
    MatDialogContent,
    MatDialogActions,
    MatDialogClose,
    MatProgressSpinner,
    MatHint,
    MatLabel,
    MatFormFieldModule,
    MatFormField,
    MatInputModule,
    FormsModule,
    MatButtonModule
],
  selector: 'app-deletesession',
  templateUrl: './deletesession.component.html'
})

export class DeleteSessionComponent
{
  sessionId: number = 0;
  session: SessionViewData | undefined;
  sessionCode: number | undefined;
  isDeleting: boolean = false;
  isDeleted: boolean = false;

  constructor(public http: HttpClient, @Inject('BASE_URL') public baseUrl: string, @Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<DeleteSessionComponent>, private readonly changeDetector: ChangeDetectorRef)
  {
    this.sessionId = data.sessionId;
    this.session = data.sessionData;
  }

  deleteSession()
  {
    if (this.http != null && this.sessionId > 0 && this.sessionCode != null && this.sessionCode > 0)
    {
      this.isDeleting = true;

      this.http.delete<boolean>(this.baseUrl + 'api/sessions/deletesession/' + this.sessionId + '/' + this.sessionCode).subscribe(
      {
        next: (result) =>
        {
          this.isDeleted = result;
          this.isDeleting = false;

          this.changeDetector.markForCheck();
        },
        error: (err) => { console.error(err); },
        complete: () =>
        {
          if (this.isDeleted)
          {
            this.dialogRef.close(true);
          }
        }
      });
    }
  }
}
