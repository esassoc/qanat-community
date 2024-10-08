import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, EventEmitter, Input, OnChanges, Output, SimpleChange } from '@angular/core';
import * as L from 'leaflet';


import { MapLayerBase } from '../map-layer-base.component';
import { WfsService } from 'src/app/shared/services/wfs.service';
import { GroupByPipe } from 'src/app/shared/pipes/group-by.pipe';
@Component({
  selector: 'water-accounts-layer',
  standalone: true,
  imports: [CommonModule, MapLayerBase],
  templateUrl: './water-accounts-layer.component.html',
  styleUrls: ['./water-accounts-layer.component.scss']
})
export class WaterAccountsLayerComponent extends MapLayerBase implements OnChanges, AfterViewInit {
  @Input() controlTitle: string = 'My Water Accounts';
  @Input({ required: true }) geographyID: number;
  @Input() waterAccountIDs: number[];
  @Input() selectedWaterAccountID: number;

  @Output() layerBoundsCalculated = new EventEmitter();
  @Output() waterAccountSelected = new EventEmitter<number>();
  
  public isLoading: boolean = false;
  public layer;

  private defaultStyle = {
    'color': '#3388ff',
    'weight': 2,
    'opacity': 0.65,
    'fillOpacity': 0.1
  };

  private highlightStyle = {
    'color': '#fcfc12',
    'weight': 2,
    'opacity': 0.65,
    'fillOpacity': 0.1
  };

  constructor(
    private wfsService: WfsService,
    private groupByPipe: GroupByPipe
  ) {
    super();
  }

  ngOnChanges(changes: any): void {
    if (changes.selectedWaterAccountID) {

      if (changes.selectedWaterAccountID.previousValue == changes.selectedWaterAccountID.currentValue) return;
      this.selectedWaterAccountID = changes.selectedWaterAccountID.currentValue;
      this.highlightSelectedtWaterAccount();

    } else if (Object.values(changes).some((x: SimpleChange) => x.firstChange === false)){
      this.updateLayer();
    }
  }

  ngAfterViewInit(): void {
    this.setupLayer();
    this.updateLayer();
  }

  updateLayer() {
    this.layer.clearLayers();

    let cql_filter = `GeographyID = ${this.geographyID}`;
    if (this.waterAccountIDs) {
      cql_filter += ` and WaterAccountID in (${this.waterAccountIDs.join(',')})`;
    }

    this.wfsService.getGeoserverWFSLayer(null, 'Qanat:AllParcels', cql_filter)
      .subscribe(response => {
        if (response.length == 0) return;
        
        const featuresGroupedByWaterAccountID = this.groupByPipe.transform(response, 'properties.WaterAccountID');

        Object.keys(featuresGroupedByWaterAccountID).forEach(waterAccountID => {
          const geoJson = L.geoJSON(featuresGroupedByWaterAccountID[waterAccountID], {
            style: this.defaultStyle
          });

          // IMPORTANT: THIS ONLY WORKS BECAUSE I'VE INSTALLED @angular/elements AND CONFIGURED THIS IN THE app.module.ts bootstrapping
          geoJson.bindPopup(`<water-account-popup-custom-element water-account-id="${waterAccountID}"></water-account-popup-custom-element>`, {
            maxWidth: 475,
            keepInView: true
          });

          geoJson.on('mouseover', (e) => {
            geoJson.setStyle({ 'fillOpacity': 0.5 });
          });
          geoJson.on('mouseout', (e) => {
            geoJson.setStyle({ 'fillOpacity': 0.1 });
          });

          geoJson.on('click', (e) => {
            this.onWaterAccountSelected(Number(waterAccountID));
          });

          geoJson.addTo(this.layer);
        });
        this.layer.addTo(this.map);
        
        const bounds = this.layer.getBounds();
        this.map.fitBounds(bounds);
        this.layerBoundsCalculated.emit(bounds);
          
        this.isLoading = false;
      });
  }

  private onWaterAccountSelected(waterAccountID: number) {
    this.selectedWaterAccountID = waterAccountID;
    this.highlightSelectedtWaterAccount();

    this.waterAccountSelected.emit(waterAccountID);
  } 

  private highlightSelectedtWaterAccount() {
    // clear styles
    this.layer.setStyle(this.defaultStyle);
    this.map.closePopup();

    // loop through the allWaterAccountsFeatureGroup
    this.layer.eachLayer((layer) => {

      const geoJsonLayers = layer.getLayers();
      if (geoJsonLayers[0].feature.properties.WaterAccountID == this.selectedWaterAccountID) {
        layer.setStyle(this.highlightStyle);
        layer.openPopup();
        this.map.fitBounds(layer.getBounds());
      }
    });
  }

  private setupLayer() {
    this.layer = L.geoJSON();
    this.initLayer();
  }
}
