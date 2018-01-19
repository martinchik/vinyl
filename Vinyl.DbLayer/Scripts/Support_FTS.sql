CREATE OR REPLACE FUNCTION make_tsvector(field TEXT)
   RETURNS tsvector AS $$
BEGIN
  RETURN (setweight(to_tsvector('english', field), 'A') || 
          setweight(to_tsvector('russian', field), 'B') || 
          setweight(to_tsvector('simple', field), 'C'));
END
$$ LANGUAGE 'plpgsql' IMMUTABLE;

CREATE INDEX IF NOT EXISTS idx_fts_resords ON "SearchItem"
  USING gin(make_tsvector("TextLine2"));
  
CREATE OR REPLACE FUNCTION fts_search(searchQuery TEXT, country TEXT)
    RETURNS TABLE("Id" uuid, "CountryCode" TEXT, "PriceFrom" numeric, "PriceTo" numeric, "RecordId" uuid, 
                  "Sell" boolean, "TextLine1" TEXT, "TextLine2" TEXT, "ShopsCount" integer, "States" TEXT) 
AS 
$$ SELECT "Id", "CountryCode", "PriceFrom", "PriceTo", "RecordId", "Sell", "TextLine1", "TextLine2", "ShopsCount", "States"
        FROM "SearchItem", to_tsquery(searchQuery) as q 
        WHERE make_tsvector("TextLine2") @@ q AND "CountryCode" = country
        ORDER BY ts_rank(make_tsvector("TextLine2"), q) DESC
$$ 
LANGUAGE SQL; 