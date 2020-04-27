import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, EMPTY, throwError } from 'rxjs';
import { take } from 'rxjs/operators';
import { catchError, map } from 'rxjs/operators';
import { Word } from '../shared/models/word';
import { TensesService } from '../services/tenses.service';

@Injectable({
  providedIn: 'root'
})
export class VerbsService {

  constructor(private http: HttpClient,
    private tensesService: TensesService) { }

  getVerbs(): Observable<Word[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    var fTenses = this.tensesService.filterTenses;
    return this.http.get<Word[]>('allVerbsForm.json', httpOptions)      
      .pipe(
        catchError(err => {
          return throwError(err);
          }
        )
      )
      .pipe(map((m) => {
        return m.map(mi => {
          return new Word(mi);
        })
          .filter(f => f.word.length > 1)
          .filter(f =>
            // Filter out unselected tenses
            fTenses == undefined || fTenses.length == 0 || fTenses.indexOf(f.tense_key) > -1
        )
        .sort(() => Math.random()-.5)
        .slice(0,10); 
      }))      
      ;
  }

}
