<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<StyledLayerDescriptor version="1.0.0" xsi:schemaLocation="http://www.opengis.net/sld StyledLayerDescriptor.xsd" xmlns="http://www.opengis.net/sld" xmlns:ogc="http://www.opengis.net/ogc" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:se="http://www.opengis.net/se">


  <NamedLayer>
    <Name>SLD Cook Book: Many color gradient</Name>
    <UserStyle>
      <Name>SLD Cook Book: Many color gradient</Name>
      <Title>SLD Cook Book: Many color gradient</Title>
      <FeatureTypeStyle>
        <Rule>
          <Name>graphic</Name>
          <TextSymbolizer>
            <Geometry>
              <ogc:Function name="boundary">
                <ogc:Function name="buffer">
                  <ogc:PropertyName>GSABoundary</ogc:PropertyName>
                  <ogc:Literal>-0.008</ogc:Literal>
                </ogc:Function>
              </ogc:Function>
            </Geometry>
            <Label>
              <ogc:PropertyName>GeographyName</ogc:PropertyName>  
            </Label>
            <LabelPlacement>
              <LinePlacement/>
            </LabelPlacement>
            <Halo>
              <Radius>3</Radius>
              <Fill>
                <CssParameter name="fill">#FFFFFF</CssParameter>
              </Fill>
            </Halo>

            <VendorOption name="followLine">true</VendorOption>
            <VendorOption name="repeat">200</VendorOption>
            <VendorOption name="spaceAround">100</VendorOption>
            <VendorOption name="maxDisplacement">90</VendorOption>
            <VendorOption name="maxAngleDelta">15</VendorOption>
            

          </TextSymbolizer>
        </Rule>
        <Rule>
          <Name>fill</Name>
          <PolygonSymbolizer>
            <Fill>
              <CssParameter name="fill"><ogc:PropertyName>GeographyColor</ogc:PropertyName></CssParameter>
              <CssParameter name="fill-opacity">.25</CssParameter>
            </Fill>
            <Stroke>
              <CssParameter name="stroke-width">2</CssParameter>
              <CssParameter name="stroke">#fff</CssParameter>
              <CssParameter name="stroke-dasharray">3</CssParameter>
              <CssParameter name="stroke-opacity">.5</CssParameter>
            </Stroke>
          </PolygonSymbolizer>
        </Rule>
      </FeatureTypeStyle>
    </UserStyle>
  </NamedLayer>
</StyledLayerDescriptor>