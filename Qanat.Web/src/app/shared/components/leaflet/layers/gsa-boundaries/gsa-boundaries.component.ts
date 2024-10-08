import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, AfterViewInit, SimpleChange } from '@angular/core';
import { environment } from 'src/environments/environment';
import * as L from 'leaflet';
import { MapLayerBase } from '../map-layer-base.component';
@Component({
  selector: 'gsa-boundaries',
  standalone: true,
  imports: [CommonModule, MapLayerBase],
  templateUrl: './gsa-boundaries.component.html',
  styleUrls: ['./gsa-boundaries.component.scss'],
})
export class GsaBoundariesComponent extends MapLayerBase implements OnChanges, AfterViewInit {

  constructor() {
    super();
  }
  @Input() geographyID: number;
  @Input() controlTitle: string = 'GSA Boundaries';
  public wmsOptions: L.WMSOptions;
  public layer;

  ngOnChanges(changes: any): void {
    if(Object.values(changes).some((x: SimpleChange) => x.firstChange === false)){
      this.updateLayer();
    }
  }

  ngAfterViewInit(): void {
    this.setupLayer();
  }

  private updateLayer(): void {
    this.updateWmsOptions();
    this.layer.setParams(this.wmsOptions);
  }

  private setupLayer() {
    this.updateWmsOptions();
    this.layer = L.tileLayer.wms(environment.geoserverMapServiceUrl + '/wms?', this.wmsOptions);

    this.initLayer();
  }

  private updateWmsOptions() {
    const cql_filter = this.geographyID ? `GeographyID = ${this.geographyID}` : '';
    this.wmsOptions = {
      layers: 'Qanat:GeographyGSABoundaries',
      transparent: true,
      format: 'image/png',
      tiled: true,
      styles: 'GSABoundaries',
    };

    if (this.geographyID) {
      this.wmsOptions.cql_filter = cql_filter;
    }
  }
}
