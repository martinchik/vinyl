
CREATE INDEX IF NOT EXISTS idx_fts_resords ON "SearchItem"
  USING gin(to_tsvector('simple', "TextLine1"));
  
CREATE OR REPLACE FUNCTION fts_search(searchQuery TEXT)
    RETURNS TABLE("Id" uuid, "PriceFrom" numeric, "PriceTo" numeric, "RecordId" uuid, "Sell" boolean, "TextLine1" TEXT, "TextLine2" TEXT) 
AS 
$$ SELECT "Id", "PriceFrom", "PriceTo", "RecordId", "Sell", "TextLine1", "TextLine2"
        FROM "SearchItem", to_tsquery(searchQuery) as q 
        WHERE to_tsvector('simple', "TextLine1") @@ q
        ORDER BY ts_rank(to_tsvector('simple', "TextLine1"), q) DESC
$$ 
LANGUAGE SQL; 


